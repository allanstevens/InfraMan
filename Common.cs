using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Specialized;
using System.DirectoryServices;

namespace AllanStevens.InfrastructureManagement
{
    public class Common
    {
        public static ArrayList AttributeValuesMultiString(string attributeName,
        string objectDn, ArrayList valuesCollection, bool recursive)
        {
            DirectoryEntry ent = new DirectoryEntry(objectDn);
            PropertyValueCollection ValueCollection = ent.Properties[attributeName];
            IEnumerator en = ValueCollection.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current != null)
                {
                    if (!valuesCollection.Contains(en.Current.ToString()))
                    {
                        valuesCollection.Add(en.Current.ToString());
                        if (recursive)
                        {
                            AttributeValuesMultiString(attributeName, "LDAP://" +
                            en.Current.ToString(), valuesCollection, true);
                        }
                    }
                }
            }
            ent.Close();
            ent.Dispose();
            return valuesCollection;
        }

        public static ArrayList Groups(string userDn, bool recursive)
        {
            ArrayList groupMemberships = new ArrayList();
            return AttributeValuesMultiString("memberOf", userDn,
                groupMemberships, recursive);
        }

        public static ArrayList Groups()
        {
            ArrayList groups = new ArrayList();
            foreach (System.Security.Principal.IdentityReference group in
                System.Web.HttpContext.Current.Request.LogonUserIdentity.Groups)
            {
                groups.Add(group.Translate(typeof
                    (System.Security.Principal.NTAccount)).ToString());
            }
            return groups;
        }

        /// <summary>
        /// Get a value from appSettings in web.config
        /// </summary>
        /// <param name="key">Key name from appSettings</param>
        /// <returns>Returns key value string</returns>
        public static string GetValueFromWebConfig(string key)
        {
            // Get the AppSettings collection.
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            return appSettings[key];
        }

        //public static ListItem[] AddCatergoryItems(string Location)
        //{
        //    switch(Location)
        //    {
        //        case "Dartford":

        //    ListItem[] returnItems = new ListItem(

        //}

        public static string SortXML(string Sort, string Order)
        {
            if (Sort == string.Empty) Sort = "name";
            if (Order == string.Empty) Order = "descending";

            return @"
                    <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
                      <xsl:strip-space elements=""*""/>
                      <xsl:output method=""xml"" indent=""yes""/>
                      <xsl:template match=""list"">
                        <xsl:copy>
                          <xsl:apply-templates select=""item"">
                            <xsl:sort select=""@" + Sort + @""" order=""" + Order + @""" data-type=""text""/>
                          </xsl:apply-templates>
                        </xsl:copy>
                      </xsl:template>
                      <xsl:template match=""@* | node()"">
                        <xsl:copy>
                          <xsl:apply-templates select=""@* | node()""/>
                        </xsl:copy>
                      </xsl:template>
                    </xsl:stylesheet>";
        }

        public static string BuildLink(string Address, BuildLinkTypes Type)
        {
            if (Address.Equals(string.Empty))
            {
                return string.Empty;
            }
            else
            {
                switch (Type)
                {
                    case BuildLinkTypes.URL:
                        return "<a href='" + Address + "' target='_blank' ><img border='0' src='image/button-url.gif' /></a>";

                    case BuildLinkTypes.RDP:
                        return "<a href='rdp.aspx?a=" + Address + "' target='_parent' ><img border='0' src='image/button-rdp.gif' /></a>";

                    case BuildLinkTypes.ILO:
                        return "<a href='https://" + Address + "' target='_blank' ><img border='0' src='image/button-ilo.gif' /></a>";

                    default:
                        return "";
                }
            }
        }
    }

    public enum BuildLinkTypes
    {
        URL,
        RDP,
        ILO
    }
}
