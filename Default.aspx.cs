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
using System.Xml;
using System.Collections.Specialized;
using System.Xml.XPath;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace AllanStevens.InfrastructureManagement
{
    public partial class _Default : System.Web.UI.Page
    {

        private string SortExpression
        {
            get
            {
                if (ViewState["_SortExpression"] == null)
                    return "name";
                else
                    return ViewState["_SortExpression"].ToString();
            }
            set {
                if (value == this.SortExpression && SortDirection == "ascending")
                    this.SortDirection = "descending";
                else
                    this.SortDirection = "ascending";
                ViewState["_SortExpression"] = value; }
        }

        private string SortDirection
        {
            get
            {
                if (ViewState["_SortDirection"] == null)
                    return "ascending";
                else
                    return ViewState["_SortDirection"].ToString();
            }
            set { ViewState["_SortDirection"] = value; }
        }

        private bool AllowEdit
        {
            get
            {
                return true;
                bool bReturn = false;

                if (ViewState["_AllowEdit"] != null)
                {
                    if (bool.TryParse(ViewState["_AllowEdit"].ToString(), out bReturn))
                    {
                        return bReturn;
                    }
                }

                List<string> sEditGroups = new List<string>(Common.GetValueFromWebConfig("EditGroups").Split(char.Parse(",")));

                foreach (string grp in Common.Groups())
                {
                    if (sEditGroups.Contains(grp)) bReturn = true;
                }

                ViewState["_AllowEdit"] = bReturn.ToString();
                return bReturn;
            }
            //set
            //{
            //    ViewState["_SortExpression"] = value;
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
         
            //client enter scriting
            ClientScript.RegisterClientScriptBlock(GetType(), "button_click", @"
                    function button_click(objTextBox,objBtnID)
                    {
                        if(window.event.keyCode==13)
                        {
                            document.getElementById(objBtnID).focus();
                            document.getElementById(objBtnID).click();
                        } 
                    }", true);

            if (!IsPostBack)
            {
                //set edit mode
                if (AllowEdit)
                {
                    GridView1.Columns[9].Visible = true;
                    GridView1.Columns[10].Visible = true;
                    lnkAddItem.Visible = true;
                }
                else
                {
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    lnkAddItem.Visible = false;
                }
                //build quicklings and dropdown
                litQuickLinks.Text = BuildQuickLinks();
                BuildCatergoryDropDown(ddlSearch.SelectedValue);
                //setup search enter js action
                txtSearch.Attributes.Add("onkeypress", "button_click(this,'" + lnkSearch.ClientID + "')");
            }


            //set style dependednt on mode
            HtmlLink styleLink = new HtmlLink();
            styleLink.Attributes.Add("rel", "stylesheet");
            styleLink.Attributes.Add("type", "text/css");
            if (lnkShowAll.Text == "Hide Columns")
                styleLink.Href = "style/style2.css";
            else
                styleLink.Href = "style/style1.css";
            Page.Header.Controls.Add(styleLink);

        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {

            txtSearch.Text = "";

            pnlAdd.Visible = false;
            GridView1.Visible = true;
            GridView1.SelectedIndex = -1;
            lnkShowAll.Visible = true;
            if (AllowEdit) lnkAddItem.Visible = true;
        }

        protected void lnkAddItem_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtIP.Text = "";
            //txtILO.Text = "";
            txtILOIP.Text = "";
            txtDescription.Text = "";
            txtURL.Text = "";
            txtGUID.Text = Guid.NewGuid().ToString();
            lblCount.Text = "";

            pnlAdd.Visible = true;
            GridView1.Visible = false;
            lnkAddItem.Visible = false;
            lnkShowAll.Visible = false;
            litNoResults.Visible = false;

            ddlLocation.SelectedIndex = 0;

            lnkSave.Text = "Add";
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {
            pnlAdd.Visible = false;
            GridView1.Visible = true;
            GridView1.SelectedIndex = -1;
            lnkShowAll.Visible = true;
            if (AllowEdit) lnkAddItem.Visible = true;
        }

        protected void lnkSave_Click(object sender, EventArgs e)
        {
            //first perform search to check if item exists before
            XmlDocument myXMLDoc = this.XmlDataSourceServerLists.GetXmlDocument();
            XmlNode nodeToCheck = myXMLDoc.SelectSingleNode("/list/item[@name='" + txtName.Text + "' and @catergory='"+ddlCatergory.SelectedValue+"' and @location='"+ddlLocation.SelectedValue+"']");
            if (nodeToCheck != null && lnkSave.Text == "Add")
            {
                //client enter scriting
                ClientScript.RegisterClientScriptBlock(GetType(), "save_alert", "alert('An item already exists with the same name, please change the name and try and save again.');", true);
            }
            else
            {
                myXMLDoc = this.XmlDataSourceServerLists.GetXmlDocument();
                XmlNode newItem = myXMLDoc.CreateNode(XmlNodeType.Element, "item", null);

                XmlAttribute xCatergory = myXMLDoc.CreateAttribute("catergory");
                XmlAttribute xLocation = myXMLDoc.CreateAttribute("location");
                XmlAttribute xName = myXMLDoc.CreateAttribute("name");
                XmlAttribute xIP = myXMLDoc.CreateAttribute("ip");
                //XmlAttribute xILO = myXMLDoc.CreateAttribute("ilo");
                XmlAttribute xILOIP = myXMLDoc.CreateAttribute("iloip");
                XmlAttribute xURL = myXMLDoc.CreateAttribute("url");
                XmlAttribute xDescription = myXMLDoc.CreateAttribute("description");
                XmlAttribute xGUID = myXMLDoc.CreateAttribute("guid");

                xCatergory.Value = ddlCatergory.SelectedValue;
                xLocation.Value = ddlLocation.SelectedValue;
                xName.Value = txtName.Text;
                xIP.Value = txtIP.Text;
                //xILO.Value = txtILO.Text;
                xILOIP.Value = txtILOIP.Text;
                xURL.Value = txtURL.Text;
                xDescription.Value = txtDescription.Text;
                xGUID.Value = txtGUID.Text;

                newItem.Attributes.Append(xGUID);
                newItem.Attributes.Append(xCatergory);
                newItem.Attributes.Append(xLocation);
                newItem.Attributes.Append(xName);
                newItem.Attributes.Append(xIP);
                //newItem.Attributes.Append(xILO);
                newItem.Attributes.Append(xILOIP);
                newItem.Attributes.Append(xURL);
                newItem.Attributes.Append(xDescription);

                if (GridView1.SelectedIndex == -1)
                {
                    XmlNode list = myXMLDoc.SelectSingleNode("list");
                    list.AppendChild(newItem);
                }
                else
                {
                    XmlNode nodeToBeReplaced = myXMLDoc.SelectSingleNode("/list/item[@guid='" + GridView1.SelectedRow.Cells[11].Text + "']");
                    nodeToBeReplaced.ParentNode.ReplaceChild(newItem, nodeToBeReplaced);
                }

                GridView1.SelectedIndex = -1;
                pnlAdd.Visible = false;
                GridView1.Visible = true;
                lnkShowAll.Visible = true;
                if (AllowEdit) lnkAddItem.Visible = true;

                XmlDataSourceServerLists.Save();
                litQuickLinks.Text = BuildQuickLinks();
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            XmlDocument myXMLDoc = this.XmlDataSourceServerLists.GetXmlDocument();           
            XmlNode nodeToBeRemoved = myXMLDoc.SelectSingleNode("/list/item[@guid='" + GridView1.Rows[e.RowIndex].Cells[11].Text + "']");

            nodeToBeRemoved.ParentNode.RemoveChild(nodeToBeRemoved);            
            XmlDataSourceServerLists.Save();
            litQuickLinks.Text = BuildQuickLinks();
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            e.ExceptionHandled = true;
        }

        protected void lnkShowAll_Click(object sender, EventArgs e)
        {
            if (lnkShowAll.Text == "Hide Columns")
            {
                lnkShowAll.Text = "Show All Columns";
                GridView1.Columns[4].Visible = false;
                GridView1.Columns[5].Visible = false;
                
            }
            else
            {
                lnkShowAll.Text = "Hide Columns";
                GridView1.Columns[4].Visible = true;
                GridView1.Columns[5].Visible = true;
            }

            //set style dependednt on mode
            HtmlLink styleLink = new HtmlLink();
            styleLink.Attributes.Add("rel", "stylesheet");
            styleLink.Attributes.Add("type", "text/css");
            if (lnkShowAll.Text == "Hide Columns")
                styleLink.Href = "style/style2.css";
            else
                styleLink.Href = "style/style1.css";
            Page.Header.Controls.Add(styleLink);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GridView1.SelectedIndex != -1)
            {
                XmlDocument myXMLDoc = this.XmlDataSourceServerLists.GetXmlDocument();
                XmlNode nodeToEdit = myXMLDoc.SelectSingleNode("/list/item[@guid='" + GridView1.SelectedRow.Cells[11].Text + "']");

                ddlLocation.SelectedValue = nodeToEdit.Attributes["location"].Value;
                txtName.Text = nodeToEdit.Attributes["name"].Value;
                txtIP.Text = nodeToEdit.Attributes["ip"].Value;
                //txtILO.Text = nodeToEdit.Attributes["ilo"].Value;
                txtILOIP.Text = nodeToEdit.Attributes["iloip"].Value;
                txtDescription.Text = nodeToEdit.Attributes["description"].Value;
                txtURL.Text = nodeToEdit.Attributes["url"].Value;
                txtGUID.Text = nodeToEdit.Attributes["guid"].Value;

                GridView1.Visible = false;
                pnlAdd.Visible = true;
                lnkShowAll.Visible = false;
                lnkAddItem.Visible = false;
                lblCount.Text = "";

                BuildCatergoryDropDown(ddlLocation.SelectedValue);
                try
                {
                    ddlCatergory.SelectedValue = nodeToEdit.Attributes["catergory"].Value;
                }
                catch
                {
                    //problem with data in xml 
                }
                lnkSave.Text = "Update";
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortExpression = e.SortExpression;
            e.Cancel = true;
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            //check its not in search mode
            if (txtSearch.Text.Trim() == "")
            {
                XmlDataSourceServerLists.XPath = TreeView1.SelectedValue.ToString();
                XmlDataSourceServerLists.Transform = Common.SortXML(SortExpression, SortDirection);
            }
            if (GridView1.Rows.Count == 0)
            {
                litNoResults.Visible = true;
            }
            else
            {
                litNoResults.Visible = false;
            }
            lblCount.Text = "Number of Item(s) " + GridView1.Rows.Count;
            lblCount.CssClass = "numberitems";
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[11].Visible = false;
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildCatergoryDropDown(ddlLocation.SelectedValue);
        }

        void BuildCatergoryDropDown(string Location)
        {
            ddlCatergory.Items.Clear();
            XmlDocument myXMLDoc = this.XmlDataSourceNavigation.GetXmlDocument();
            XmlNodeList nodesToDisplay = myXMLDoc.SelectNodes("/menu/section[@name='" + Location + "']/item");
            foreach (XmlNode node in nodesToDisplay)
            {
                ddlCatergory.Items.Add(new ListItem(node.Attributes["name"].Value.ToString(), node.Attributes["name"].Value.ToString()));
            }
        }

        string BuildQuickLinks()
        {
            StringBuilder rtn = new StringBuilder();

            XmlDocument myXMLDoc = this.XmlDataSourceServerLists.GetXmlDocument();
            XmlNodeList nodesToDisplay = myXMLDoc.SelectNodes("/list/item[@location='Other'][@catergory='Monitoring']");
            foreach (XmlNode node in nodesToDisplay)
            {
                rtn.AppendLine("<a class=\"usefullinks\" href=\"" + node.Attributes["url"].Value.ToString() + "\" target=\"_blank\" >" + node.Attributes["name"].Value.ToString() + "</a>");
                rtn.AppendLine("<br/>");
            }

            if (rtn.Length == 0) return "";
            else
            {
                return "<div id=\"sidemenu\"><div id=\"menu\"><div class=\"box_tl\"></div><div class=\"box_tr\"></div>" +
                        "<div class=\"box_title\">Monitoring</div>" +
                        "<div class=\"box_content\">" + rtn.ToString() + "</div>" +
                        "<div class=\"box_bl\"></div><div class=\"box_br\"></div></div>";
            }
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            if(TreeView1.SelectedNode != null) TreeView1.SelectedNode.Selected = false;
            XmlDataSourceServerLists.XPath = "/list/item" + ddlSearch.SelectedValue + "[contains(translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'" + txtSearch.Text.ToLower() + "') or contains(translate(@description, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'" + txtSearch.Text.ToLower() + "') ]";
        }

    }
}
