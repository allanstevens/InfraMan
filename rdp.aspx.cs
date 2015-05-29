using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace AllanStevens.InfrastructureManagement
{
    public partial class rdp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            {
                if (Request.QueryString.Count != 0)
                {
                    Response.Clear();
                    Response.ContentType = "application/rdp";
                    Response.AddHeader("Content-Disposition", "filename=\"connection.rdp\"");
                    Response.ContentEncoding = System.Text.Encoding.UTF8;

                    Response.Write("screen mode id:i:2\n");
                    Response.Write("desktopwidth:i:1280\n");
                    Response.Write("desktopheight:i:1024\n");
                    Response.Write("session bpp:i:16\n");
                    //Response.Write("winposstr:s:0,1,1334,96,2556,966\n");
                    Response.Write("compression:i:1\n");
                    Response.Write("keyboardhook:i:2\n");
                    Response.Write("displayconnectionbar:i:1\n");
                    Response.Write("disable wallpaper:i:1\n");
                    Response.Write("disable full window drag:i:1\n");
                    Response.Write("allow desktop composition:i:0\n");
                    Response.Write("allow font smoothing:i:0\n");
                    Response.Write("disable menu anims:i:1\n");
                    Response.Write("disable themes:i:0\n");
                    Response.Write("disable cursor setting:i:0\n");
                    Response.Write("bitmapcachepersistenable:i:1\n");
                    Response.Write("full address:s:" + Request.QueryString[0] + "\n");
                    Response.Write("audiomode:i:0\n");
                    Response.Write("redirectprinters:i:1\n");
                    Response.Write("redirectcomports:i:0\n");
                    Response.Write("redirectsmartcards:i:1\n");
                    Response.Write("redirectclipboard:i:1\n");
                    Response.Write("redirectposdevices:i:0\n");
                    Response.Write("autoreconnection enabled:i:1\n");
                    Response.Write("authentication level:i:0\n");
                    Response.Write("prompt for credentials:i:0\n");
                    Response.Write("negotiate security layer:i:1\n");
                    Response.Write("remoteapplicationmode:i:0\n");
                    Response.Write("alternate shell:s:\n");
                    Response.Write("shell working directory:s:\n");
                    Response.Write("gatewayhostname:s:\n");
                    Response.Write("gatewayusagemethod:i:4\n");
                    Response.Write("gatewaycredentialssource:i:4\n");
                    Response.Write("gatewayprofileusagemethod:i:0\n");
                    Response.Write("promptcredentialonce:i:1\n");
                    Response.Write("drivestoredirect:s:C:;\n");

                    Response.End();
                }
            }
        }
    }
}
