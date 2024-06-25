<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/351748711/21.1.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T985523)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# How to Bind Data Grid to a gRPC Service

This example demonstrates how to use the <a href="https://documentation.devexpress.com/WPF/DevExpress.Xpf.Data.InfiniteAsyncSource.class">InfiniteAsyncSource</a> to bind the <a href="https://documentation.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl.class">GridControl</a> to a <a href="https://grpc.io/">gRPC</a> service.
 
Follow the steps below to run the example:

1. Build the projects in the solution.
2. Start the test server. To do this, run the following .exe file: `\how-to-bind-to-grpc\CS\IssuesData.Server\bin\Debug\netcoreapp3.1\IssuesData.Server.exe`.
3. Run the `InfiniteAsyncSource.GRPC` project in Visual Studio.

For information on how to use virtual sources, refer to the following tutorial: [How to Use Virtual Sources](https://docs.devexpress.com/WPF/120194/controls-and-libraries/data-grid/bind-to-data/bind-to-any-data-source-with-virtual-sources/how-to-use-virtual-sources).

#### Note:
> This example does not have a VB implementation because Visual Basic is not supported by gRPC (see: [Supported languages](https://grpc.io/docs/languages/)).
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=how-to-bind-to-grpc&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=how-to-bind-to-grpc&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
