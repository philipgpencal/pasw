# Phil's Assignment Scalable Web

API Rest to diff data json Base64 encripted. 
Built using asp.net core api 2.2 on Microsoft Visual Studio 2019.

<b>Usage:</b>

The documentation is based on swagger, you'll see 3 web methods available:

/v1/Diff/{id}/left<br />
/v1/Diff/{id}/right<br />
/v1/Diff/{id}

Required Parameters:
{id} - long - The identity of a diff data structure, used to insert diff data for both sides, and retrieve the results later.

For POST operations: 
data - string - you must to include the comparable data sending the Base64 encrpted hash using the parameter "data"
Examples: "IHsgIm5hbWUiOiJQZXRlIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==" 

<b>Results Example:</b>

{
    "id": 1,<br />
    "left": "IHsgIm5hbWUiOiJQZXRlIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==",<br />
    "right": "IHsgIm5hbWUiOiJKb2huIiwgImFnZSI6MzAsICJjYXIiOm51bGwgfQ==",<br />
    "equal": false,<br />
    "sameSize": true,<br />
    "diffInsights": ["Difference detected, starting at offset 16 with length of 5."]<br />
}