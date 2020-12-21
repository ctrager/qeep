#pragma checksum "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "eeb039506b20bb5b59e67bd2cfe9b13cabb4f348"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(keep.Pages.Shared.Pages_Shared__PlainDataTablePartial), @"mvc.1.0.view", @"/Pages/Shared/_PlainDataTablePartial.cshtml")]
namespace keep.Pages.Shared
{
    #line hidden
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using keep;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using System.Data.SqlClient;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using System.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using System.Data.Common;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using System.Collections.Generic;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using Microsoft.AspNetCore.Mvc;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using Microsoft.AspNetCore.Mvc.RazorPages;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "/home/corey/github/keep_server/Pages/_ViewImports.cshtml"
using Microsoft.AspNetCore.Mvc.Rendering;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"eeb039506b20bb5b59e67bd2cfe9b13cabb4f348", @"/Pages/Shared/_PlainDataTablePartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6aea85d16373da7f52ed5e0bde6d1baf48aa57c7", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Shared__PlainDataTablePartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
  
    if (Model.error is not null)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"flash_err\">");
#nullable restore
#line 4 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                          Write(Model.error);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n");
#nullable restore
#line 5 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
    }
    if (Model.success is not null)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"flash_msg\">");
#nullable restore
#line 8 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                          Write(Model.success);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n");
#nullable restore
#line 9 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 12 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
  
    if (@Model.dt is not null)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"data_table_div\">\n            <div class=\"info_text just_left\">");
#nullable restore
#line 16 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                                        Write(Model.dt.Rows.Count);

#line default
#line hidden
#nullable disable
            WriteLiteral(" rows</div>\n\n            <table id=\"results_table\" class=\"dt\">\n");
            WriteLiteral("                    <tr>\n");
#nullable restore
#line 21 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                          
                            for (int i = 0; i < Model.dt.Columns.Count; i++)
                            {


#line default
#line hidden
#nullable disable
            WriteLiteral("                                <th>");
#nullable restore
#line 25 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                               Write(Model.dt.Columns[i].ColumnName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\n");
#nullable restore
#line 26 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                            }
                        

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </tr>\n");
#nullable restore
#line 29 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                    for (int row = 0; row < Model.dt.Rows.Count; row++)
                    {
                        DataRow dr = Model.dt.Rows[row];

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <tr>\n");
#nullable restore
#line 33 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                              
                                for (int col = 0; col < Model.dt.Columns.Count; col++)
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <td>");
#nullable restore
#line 36 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                                   Write(dr[col]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n");
#nullable restore
#line 37 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                                }
                            

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                        </tr>\n");
#nullable restore
#line 41 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
                    }

                

#line default
#line hidden
#nullable disable
            WriteLiteral("            </table>\n        </div>\n");
#nullable restore
#line 46 "/home/corey/github/keep_server/Pages/Shared/_PlainDataTablePartial.cshtml"
    }

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
