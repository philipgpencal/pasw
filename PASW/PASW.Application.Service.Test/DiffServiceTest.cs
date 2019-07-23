using Moq;
using PASW.Domain.Entity.Enum;
using PASW.Domain.Interface.Repository;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using PASW.Application.Service.Test.Factory;
using PASW.Util.Exceptions;
using PASW.Domain.Entity;

namespace PASW.Application.Service.Test
{
    public class DiffServiceTest
    {
        private readonly DiffService target;
        private readonly Mock<IDiffRepository> comparisonRepositoryMock;

        public DiffServiceTest()
        {
            comparisonRepositoryMock = new Mock<IDiffRepository>();
            target = new DiffService(comparisonRepositoryMock.Object);
        }



        [Fact]
        public void PostDiffEntry_WHEN_id_lower_or_equal_zero_SHOULD_throw_argument_out_of_range_exception()
        {
            Func<Task> postDiffEntry = async () => await target.PostDiffEntry(-1, Side.Left, null);

            postDiffEntry.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void PostDiffEntry_WHEN_data_null_SHOULD_throw_argument_null_exception()
        {
            Func<Task> postDiffEntry = async () => await target.PostDiffEntry(1, Side.Left, null);

            postDiffEntry.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async Task PostDiffEntry_WHEN_request_is_new_SHOULD_call_repository_insert()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1)).ReturnsAsync(() => null);

            await target.PostDiffEntry(1, Side.Left, "someValue");

            comparisonRepositoryMock.Verify(repository => repository.Insert(It.IsAny<ComparisonRequest>()), Times.Once);
        }

        [Fact]
        public async Task PostDiffEntry_WHEN_request_is_not_new_SHOULD_call_repository_update()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1)).ReturnsAsync(ComparisonRequestFactory.GetSingleSideNull(1, Side.Left, "someValue"));

            await target.PostDiffEntry(1, Side.Left, "someValue");

            comparisonRepositoryMock.Verify(repository => repository.Update(It.IsAny<ComparisonRequest>()), Times.Once);
        }

        [Fact]
        public void Compare_WHEN_id_lower_or_equal_zero_SHOULD_throw_argument_out_of_range_exception()
        {
            Func<Task> compare = async () => await target.Diff(-1);

            compare.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Compare_WHEN_left_not_null_and_right_null_SHOULD_throw_insuficient_data_for_comparison_exception()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetSingleSideNull(1, Side.Right, "someValue"));

            Func<Task> compare = async () => await target.Diff(1);
            compare.ShouldThrow<DataComparisonNotEnoughException>();
        }

        [Fact]
        public void Compare_WHEN_left_null_and_right_not_null_SHOULD_throw_insuficient_data_for_comparison_exception()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetSingleSideNull(1, Side.Left, "someValue"));

            Func<Task> compare = async () => await target.Diff(1);
            compare.ShouldThrow<DataComparisonNotEnoughException>();
        }

        [Fact]
        public void Compare_WHEN_left_null_and_right_null_SHOULD_throw_insuficient_data_for_comparison_exception()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetSingleSideNull(1, Side.Left, "someValue"));

            Func<Task> compare = async () => await target.Diff(1);
            compare.ShouldThrow<DataComparisonNotEnoughException>();
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_are_empty_SHOULD_return_equal()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, string.Empty, string.Empty));

            var result = await target.Diff(1);

            result.Id.Should().Be(1);
            result.Left.Should().Be(string.Empty);
            result.Right.Should().Be(string.Empty);
            result.Equal.Should().Be(true);
            result.SameSize.Should().Be(true);
            result.DiffInsights.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_different_all_SHOULD_return_that()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "bcc", "baba"));

            var result = await target.Diff(1);

            result.Equal.Should().Be(false);
            result.SameSize.Should().Be(false);
            result.DiffInsights.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_different_size_SHOULD_return_that()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "aaa", "aaaaa"));

            var result = await target.Diff(1);

            result.Equal.Should().Be(false);
            result.SameSize.Should().Be(false);
            result.DiffInsights.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_equals_SHOULD_return_that()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "aaaaa", "aaaaa"));

            var result = await target.Diff(1);

            result.Id.Should().Be(1);
            result.Left.Should().Be("aaaaa");
            result.Right.Should().Be("aaaaa");
            result.Equal.Should().Be(true);
            result.SameSize.Should().Be(true);
            result.DiffInsights.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_different_but_same_size_SHOULD_return_that()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "bbbbb", "aaaaa"));

            var result = await target.Diff(1);

            result.Equal.Should().Be(false);
            result.SameSize.Should().Be(true);
            result.DiffInsights.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_different_but_same_size_SHOULD_return_one_insight()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "11110", "11111"));

            var result = await target.Diff(1);

            result.Equal.Should().Be(false);
            result.SameSize.Should().Be(true);
            result.DiffInsights.Should().HaveCount(1);
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_different_but_same_size_SHOULD_return_one_insights()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "11110", "11111"));

            var result = await target.Diff(1);

            result.Equal.Should().Be(false);
            result.SameSize.Should().Be(true);
            result.DiffInsights.Should().HaveCount(1);
        }

        [Fact]
        public async Task Compare_WHEN_left_and_right_different_but_same_size_SHOULD_return_two_insights()
        {
            comparisonRepositoryMock.Setup(m => m.GetById(1))
                .ReturnsAsync(ComparisonRequestFactory.GetValid(1, "00000000000", "00123000450"));

            var result = await target.Diff(1);

            result.Equal.Should().Be(false);
            result.SameSize.Should().Be(true);
            result.DiffInsights.Should().HaveCount(2);
        }
    }
}
