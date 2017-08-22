using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AspMap;
using AspMap.Web;
using AspMap.Web.Extensions;


public partial class DataExplorer : System.Web.UI.Page
{


    private MyDataBase dbClsAcc = new MyDataBase();
    private MySessions sesn = new MySessions();
   // private MyDataBase dbAccCls = new MyDataBase();
   // private ExportTools expToolsCls = new ExportTools();



    private static string amLicenseKey, gmLicenseKey, mapRootDir;
    private DataTable allData = new DataTable();
    private DataTable dtAllLayers = null;

  //  private DataTable dtg = null;

    private AspMap.Recordset oRs = null;
    private CheckBox[] myboxes = new CheckBox[20];
    private CheckBox[] ActiveBox = new CheckBox[20];
    private int itemidx = 0;

    public AspMap.Layer layer = null;
    //======================//



    protected DataTable dtMapLayers
    {
        get
        {
            object sObj = Session["dtMapLayers"];
            return (sObj != null) ? (DataTable) sObj : InitMapLayerTable();
        }
        set { Session["dtMapLayers"] = value; }
    }


    protected DataTable InitMapLayerTable()
    {
        DataTable dt = new DataTable("MapLayers");

        dt.Columns.Add("LayerOrgName", typeof(string));
        dt.Columns.Add("LayerName", typeof(string));
        dt.Columns.Add("LegendName", typeof(string));
        dt.Columns.Add("ShapeFilePath", typeof(string));
        dt.Columns.Add("LabelField", typeof(string));
        dt.Columns.Add("LayerActiveMode", typeof(bool));

        return dt;
    }



    protected void Page_Load(object sender, EventArgs e)
    {
       

        hlMetadataViewer.Attributes.Add("onclick", "callpopup()");
        atImgButton.Attributes.Add("onclick", "attcallpopup()");
       

        //=============== start ================//

        if (IsPostBack)
        {
            addLayerName();
        }


        /*
        string tvSelVal = tvDataSource.SelectedValue;


        ////Tabular

        if (tvSelVal != "")
        {
            string[] words = tvSelVal.Split('_');
            string groupId = words[0];
            string typeId = words[1];
            string sourceId = words[2];
            string dataType = dbClsAcc.GetDataTypeName(groupId, typeId, sourceId);

            if ((groupId != "") && (typeId != "") && (sourceId != "") && (dataType == "Shape"))
            {
                multiView.ActiveViewIndex = 0;
                multiView.GetActiveView();
               // multiView.Dispose();
              //  multiView.SetActiveView(shapeView);

            }
            else if ((groupId != "") && (typeId != "") && (sourceId != "") && (dataType == "Tabular"))
            {
                multiView.ActiveViewIndex = 1;
                multiView.GetActiveView();
                // multiView.Dispose();
                // multiView.SetActiveView(tableView);

            }
            else if ((groupId != "") && (typeId != "") && (sourceId != "") && (dataType == "pdf"))
            {

            }
        }


        */

        //================ end ===============//

        // if(tvDataSource.s)

         // if (MyDataBase.dataType != "Tabular" || MyDataBase.dataType=="")
        
        //  multiView.ActiveViewIndex = 0;


        
    
        {

            AddGoogleMapsLayer();

            if (GoogleMapddl.SelectedValue == "Normal" || GoogleMapddl.SelectedValue == "Physical" ||
                GoogleMapddl.SelectedValue == "Satellite" || GoogleMapddl.SelectedValue == "Hybrid")
            {
                myMap.BackgroundLayer.Visible = true;
            }
            else
            {
                if (myMap.BackgroundLayer != null)
                    myMap.BackgroundLayer.Visible = false;
            }


            //if (GoogleMapddl.SelectedValue == "Normal")
            //{
            //    GoogleMapddl.SelectedValue = "Normal";
            //    myMap.BackgroundLayer.Visible = true;

            //}

            myMap.ImageFormat = ImageFormat.Png;



        }

        


        //if (MyDataBase.dataType == "Tabular")
        //{
        //    if (myMap.BackgroundLayer != null)
        //        myMap.BackgroundLayer.Visible = false;
        //}


        if (!IsPostBack)
        {
            FillRootNode();

            try
            {
                sesn.currentUrl = Request.Url.AbsoluteUri;
                var masterPage = this.Master;
                if (masterPage != null)
                {
                    try
                    {
                        ((Label) this.Master.FindControl("lblPageTitle")).Text = "Data Explorer";
                        ((System.Web.UI.HtmlControls.HtmlGenericControl) this.Master.FindControl("A2")).Attributes.Add(
                            "Class", "current");

                    }
                    catch
                    {
                    }
                }

            }
            catch
            {
            }


            //========= Map Property=============//

            myMap.ScaleBar.Visible = true;
            myMap.ScaleBar.Position = ScaleBarPosition.BottomRight;
            myMap.MapUnit = MeasureUnit.Meter;
            myMap.ScaleBar.BarUnit = UnitSystem.Metric;
            myMap.CoordinateSystem = CoordSystem.WGS1984;
            myMap.ImageFormat = AspMap.ImageFormat.Png;

           // AddMapLocator();


            try
            {
               
                mapRootDir = ConfigurationManager.AppSettings["ShapeFilesDir"].ToString();

                mapRootDir = (string.IsNullOrEmpty(mapRootDir.Trim()) ? mapRootDir : @"ShapeFiles/");
                mapRootDir = MapPath(mapRootDir);

            }
            catch
            {

            }

            //============start =====//
            // if (!IsPostBack)
            {

                try
                {
                    dtMapLayers = null;

                }
                catch
                {
                }


                if (dtMapLayers != null)
                    dtMapLayers.Rows.Clear();



                myMap.CenterAt(myMap.CoordinateSystem.FromWgs84(90, 23.6));
                myMap.ZoomLevel = 7;


            }



            //  MyDbAccClass.FillDropDownListAllCh(ddlfordiv, "SELECT DISTINCT forestdiv FROM tblgreenbelttotal ORDER BY forestdiv", "forestdiv", "forestdiv");


            //            sqlString = @"SELECT DATAGROUPID, DATAGROUP FROM TBLDATAGROUP ORDER BY DATAGROUPID, DATAGROUP";

            //            sqlString = "SELECT DATATYPEID, DATATYPE FROM TBLDATATYPE WHERE DATAGROUPID='" + DataGroup + "' ORDER BY DATATYPEID, DATATYPE";


            //using (DataTable dt = dbClsAcc.GetDataGroups())
            //{
            //    FillDropDownList(ddlDataGroup, dt, "DATAGROUP", "DATAGROUPID");
            //}


            //==================== start====================//
            MyDataBase.FillDropDownListDECCMA(ddlDataGroup, "SELECT DATAGROUPID, DATAGROUP FROM TBLDATAGROUP ORDER BY DATAGROUPID, DATAGROUP", "DATAGROUP", "DATAGROUPID");

            
            //===================== End ===================//
            

            loadInitialMapInfo();


        }
        

        
        {
              
                AddMapLocator();
        }




        //===================  start===================//

        /*

        string tvSelVal = tvDataSource.SelectedValue;


        ////Tabular

        if (tvSelVal != "")
        {
            string[] words = tvSelVal.Split('_');
            string groupId = words[0];
            string typeId = words[1];
            string sourceId = words[2];
            string dataType = dbClsAcc.GetDataTypeName(groupId, typeId, sourceId);

            if ((groupId != "") && (typeId != "") && (sourceId != "") && (dataType == "Tabular" || dataType == "TimeSeries"))
            {

                multiView.ActiveViewIndex = 1;
                multiView.GetActiveView();


                // multiView.Dispose();
                //  multiView.SetActiveView(shapeView);

                //TimeSeries

                //dataType


                if ((dataType == "Tabular" || dataType == "TimeSeries") && myMap.BackgroundLayer != null)
                {


                   // myMap.RemoveAllLayers();
                   // myMap.BackgroundLayer.Visible = false;


               

                }
                else if ((dataType == "Tabular" || dataType == "TimeSeries") && myMap.BackgroundLayer == null)
                    return;




            }

        }



        */


        //==================== End ==================//





    }


    public void addLayerName()
    {

        tblLayers.Rows.Clear();



        try
        {
            using (DataTable dtMapLayersTemp = dtMapLayers)
            {
                for (int l = dtMapLayersTemp.Rows.Count - 1; l >= 0; l--)
                {

                    string mapLayerId = dtMapLayersTemp.Rows[l]["LayerName"].ToString();
                    string lgndField = dtMapLayersTemp.Rows[l]["LegendName"].ToString();
                    string shapeSource = dtMapLayersTemp.Rows[l]["ShapeFilePath"].ToString();

                    layer = myMap[mapLayerId];
                    string mapLayerName = layer.Description;

                    bool layActiveMode = Convert.ToBoolean(dtMapLayersTemp.Rows[l]["LayerActiveMode"].ToString());



                    if (layActiveMode == true)
                    {
                        TableRow rw = new TableRow();
                        TableCell cel = new TableCell();
                        CheckBox box = new CheckBox();
                        box.Visible = true;
                        box.ID = "chkBox" + (layer.Name).ToString();
                        box.Checked = layer.Visible;
                        box.AutoPostBack = true;
                        //  box.CheckedChanged += new EventHandler(chkBox_CheckedChanged);

                        box.Enabled = false;

                        cel.HorizontalAlign = HorizontalAlign.Right;

                        myboxes[itemidx] = box;
                        itemidx = itemidx + 1;
                        cel.Controls.Add(box);
                        rw.Cells.Add(cel);
                        TableCell cel1 = new TableCell();

                      
                        cel1.Text = layer.Description;
                    

                        cel1.HorizontalAlign = HorizontalAlign.Left;
                        rw.Cells.Add(cel1);
                        rw.Height = 5;
                        tblLayers.Rows.Add(rw);




                        SetMapColorDec(layer, shapeSource, lgndField, "", 0, true, mapLayerId, true);


                    }


                }


            }
        }
        catch (Exception exception)
        {
            string ex = exception.ToString();
        }


    }



    private void FillRootNode()
    {

       
        string[] rootName = new string[] { "Data Source" };
        string[] rootId = new string[] { "0" };

     
        {
            TreeNode r = new TreeNode();
            r.Text = rootName[0].ToString();
            r.Value = rootId[0].ToString();

            r.PopulateOnDemand = true;
            r.SelectAction = TreeNodeSelectAction.None;

            tvDataSource.Nodes.Add(r);
            tvDataSource.CollapseAll();
        }

        

    }



    //protected void FillDropDownList(DropDownList ddlObject, DataTable dtList, string DisMember, string ValMember)
    //{
    //    ddlObject.Items.Clear();
    //    ddlObject.DataSource = dtList;
    //    ddlObject.DataTextField = DisMember;
    //    ddlObject.DataValueField = ValMember;
    //    ddlObject.DataBind();
    //    ddlObject.Items.Insert(0, new ListItem("<-------- Select One ------->", ""));
    //}



    protected void ddlDataGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        //======= start ========//

        //========start=========//
        //dvAllData.DataSource = null;
        //dvAllData.DataBind();
        //dvAllData.Visible = false;
       // dtg = null;
        //===== end===========//

        //=========  end =========//




        ddlDataType.Items.Clear();
        

        //if (ddlDataGroup.SelectedValue != "")
        //{
        //    using (DataTable dt = dbClsAcc.GetDataTypes(ddlDataGroup.SelectedValue))
        //    {
        //        FillDropDownList(ddlDataType, dt, "DATATYPE", "DATATYPEID");
        //    }
        //}



        //=========== start =============//

        MyDataBase.FillDropDownListDECCMA(ddlDataType, "SELECT DATATYPEID, DATATYPE FROM TBLDATATYPE WHERE DATAGROUPID='" + ddlDataGroup.SelectedValue + "' ORDER BY DATATYPEID, DATATYPE", "DATATYPE", "DATATYPEID");


        //============= end ===========//


    }



    protected void ddlDataType_SelectedIndexChanged(object sender, EventArgs e)
    {
      //  sesn.dTblName = "";


        //================ start ===============//

        //   multiView.ActiveViewIndex = 2;





        //========= ====== end ===============//

        //========start=========//
        //dvAllData.DataSource = null;
        //dvAllData.DataBind();
        //dvAllData.Visible = false;
       // dtg = null;
        //===== end===========//


        //  UpdateMapInfo();

        tvDataSource.Nodes.Clear(); 

        

        if (!string.IsNullOrEmpty(ddlDataType.SelectedValue))
        {
            using (DataTable dtChilds = dbClsAcc.GetChildNodes(ddlDataGroup.SelectedValue, ddlDataType.SelectedValue))
            {
                if (dtChilds != null && dtChilds.Rows.Count > 0)
                {
                    TreeNode headNode = new TreeNode();

                    if (dtChilds.Rows.Count > 0)
                    {
                        headNode.Text = "Data Source";
                        headNode.Value = "";
                        headNode.Expand();
                        tvDataSource.Nodes.Add(headNode);
                    }
                    else
                    {
                        headNode.Text = "No Data Source";
                        tvDataSource.Nodes.Add(headNode);
                        return;
                    }

                    foreach (DataRow dr in dtChilds.Rows)
                    {
                        TreeNode childNode = new TreeNode();
                        
                        childNode.Value = ddlDataGroup.SelectedValue+"_"+ ddlDataType.SelectedValue+"_"+ dr[0].ToString();

                        childNode.Text = dr[1].ToString();

                        headNode.ChildNodes.Add(childNode);
                    }
                }
            } 



        }

        
    }


    protected void tvDataSource_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {

        //switch (e.Node.Depth)
        //{
        //    case 0:
        //      //  PopulateDataSrc(e.Node);
                
        //        break;

          

        //    default:
        //        break;
        //}


    }



    private void PopulateDataSrc(TreeNode folderNode)
    {


        using (DataTable dtChilds = dbClsAcc.GetChildNodes(ddlDataGroup.SelectedValue, ddlDataType.SelectedValue))
        {
            if (dtChilds != null && dtChilds.Rows.Count > 0)
            {
               
                foreach (DataRow dr in dtChilds.Rows)
                {
                    TreeNode childNode = new TreeNode();

                  

                    childNode.Value = ddlDataGroup.SelectedValue + "_" + ddlDataType.SelectedValue + "_" + dr[0].ToString();

                    childNode.Text = dr[1].ToString();

                    folderNode.ChildNodes.Add(childNode);
                }
            }
        }





        string fnValue = folderNode.Value.ToString();
        string fnText = folderNode.Text;

        // if (fnText == "Picture")
        //  {





        //  string sn = reader["SECTOR"].ToString();
        //  string siStr = reader["SECTORID"].ToString();

        /*
        TreeNode newNode = new TreeNode();
        newNode.Text = "Data Source";
        newNode.Value = "Data Source";
        newNode.PopulateOnDemand = true;
        newNode.SelectAction = TreeNodeSelectAction.None;
        folderNode.ChildNodes.Add(newNode);

        */

        //}






    }




    public void loadInitialMapInfo()
    {
       
        using ( dtAllLayers = dbClsAcc.GetMapInfo())
        {
            
            foreach (DataRow dr in dtAllLayers.Rows)
            {
                string mapLayerId = dr["DATAGROUPID"].ToString()+"_"+ dr["DATATYPEID"].ToString()+"_"+ dr["DATASOURCEID"].ToString();
                
                string shpPath = dr["DATASOURCEPATH"].ToString();
                shpPath = mapRootDir + shpPath;
                  
                using (DataTable dtMapLayersTemp = dtMapLayers)
                 {
                            try
                            {
                                DataRow dmr = dtMapLayers.NewRow();

                       

                                 dmr["LayerOrgName"] = dr["DATASOURCE"].ToString();
                                 dmr["LayerName"] = mapLayerId;

                                 dmr["LegendName"] = dr["CLASSIFYFIELD"].ToString();

                                 dmr["ShapeFilePath"] = shpPath;
                                 dmr["LabelField"] = dr["LABELFIELD"].ToString();
                                 dmr["LayerActiveMode"] = false;


                       
                                if (!IsLayerDataRowExist(mapLayerId))
                                    dtMapLayersTemp.Rows.Add(dmr.ItemArray);

                                dtMapLayersTemp.AcceptChanges();
                            }
                            catch (Exception)
                            {
                            }
                              dtMapLayers = dtMapLayersTemp;

                 }


              }
        }

        AddMapLayersDec(dtMapLayers);

    }



   

    protected bool IsLayerDataRowExist(string mapLayerId)
    {
        //LayerName=maplayeriD
        if (dtMapLayers.Rows.Count < 1) return false;

        for (int l = dtMapLayers.Rows.Count - 1; l >= 0; l--)
        {
            if (!dtMapLayers.Rows[l]["LayerName"].ToString().Equals(mapLayerId)) continue;

            return true;
        }

        return false;
    }




    void AddGoogleMapsLayer()
    {

        string GML = ConfigurationManager.AppSettings["GoogleMapLicenseKey"].ToString();

        GoogleMapsLayer gml = new GoogleMapsLayer(GML);


        if (GoogleMapddl.SelectedValue == "Normal")
        {
            gml.MapType = GoogleMapType.Normal;
        }
        else if (GoogleMapddl.SelectedValue == "Physical")
        {
            gml.MapType = GoogleMapType.Physical;
        }
        else if (GoogleMapddl.SelectedValue == "Satellite")
        {
            gml.MapType = GoogleMapType.Satellite;
        }
        else if (GoogleMapddl.SelectedValue == "Hybrid")
        {
            gml.MapType = GoogleMapType.Hybrid;
        }
        else
        {
            gml.MapType = GoogleMapType.Normal;
        }


        gml = new GoogleMapsLayer(GML, gml.MapType);

       

        myMap.BackgroundLayer = gml;
    }


   
    protected void GoogleMapddl_SelectedIndexChanged(object sender, EventArgs e)
    {


        if (GoogleMapddl.SelectedValue == "Normal" || GoogleMapddl.SelectedValue == "Physical" || GoogleMapddl.SelectedValue == "Satellite" || GoogleMapddl.SelectedValue == "Hybrid")
        {

            myMap.BackgroundLayer.Visible = true;

        }
        else
        {
            if (myMap.BackgroundLayer != null) myMap.BackgroundLayer.Visible = false;

        }


    }



    protected void Page_PreRender(object sender, System.EventArgs e)
    {
       
        try
        {
            // bool isActiveLayer = (dtMapLayers != null && dtMapLayers.Select("LayerActiveMode=TRUE").Length > 0);

            bool isActiveLayer = true;
            UpdateMapLocator(isActiveLayer);
        }
        catch
        {
        }

      //  if (dataType == "Tabular" || dataType == "TimeSeries")



     
        if ((MyDataBase.dataType == "Tabular" || MyDataBase.dataType == "TimeSeries") && myMap.BackgroundLayer != null)
        {

            /*
            myMap.RemoveAllLayers();
            myMap.BackgroundLayer.Visible = false; 
            
            */


            myMap.BackgroundLayer.Visible = false;


            myMap.Callouts.Clear();
            myMap.Markers.Clear();
           
            myMap.MapShapes.Clear();
            myMap.RemoveAllLayers();

            myMap.BackgroundLayer = null;






        }
        else if ((MyDataBase.dataType == "Tabular" || MyDataBase.dataType == "TimeSeries") && myMap.BackgroundLayer == null)

            return;

        




            // MyDataBase.dataType  == Tabular
            //  if (MyDataBase.dataType == "Tabular")
            //   multiView.ActiveViewIndex = 1;


    }

    private void UpdateMapLocator(bool isAnyLayer)
    {
        myMapLocator.MapShapes.Clear();

        if (!isAnyLayer) return;


        CoordSystem cr = new CoordSystem();

        if (myMap != null && myMap.CoordinateSystem != null)
        {
            cr = myMap.CoordinateSystem;
            myMapLocator.CoordinateSystem = cr;

        }

        AspMap.Rectangle mapExt = myMapLocator.Extent;
        mapExt = myMapLocator.CoordinateSystem.TransformRectangle(cr, mapExt);
        myMapLocator.Extent = mapExt;

        AspMap.Rectangle maplocExt = myMap.Extent;
        maplocExt = myMapLocator.CoordinateSystem.TransformRectangle(cr, maplocExt);

        MapShape mapShape = myMapLocator.MapShapes.Add(maplocExt);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;
        mapShape.Symbol.FillStyle = FillStyle.Invisible;



    }



   
    protected void AddMapLayersDec(DataTable mapLayears)
    {
        myMap.Callouts.Clear();
        myMap.Markers.Clear();
        myMap.MapShapes.Clear();
        myMap.RemoveAllLayers();
        

        foreach (DataRow mapLayer in mapLayears.Rows)
        {
            try
            {

                AddMapLayerDec(mapLayer);
                
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
        }



        if (myMap == null || myMap.LayerCount < 1) return;

        

    }

    protected void AddMapLayerDec(DataRow mapLayer, AspMap.Layer layer = null)
    {
       
        string mapLayerId = mapLayer["LayerName"].ToString();
        string lyrName = mapLayer["LayerOrgName"].ToString();
        string shpPath = mapLayer["ShapeFilePath"].ToString();
        string lblField = mapLayer["LabelField"].ToString();
        string legnField = mapLayer["LegendName"].ToString();
        string activemode = mapLayer["LayerActiveMode"].ToString();



        bool isActive = Convert.ToBoolean(activemode);



        bool isNewLayer = (layer == null);

        if (isNewLayer)
        {
            try
            {
                if (string.IsNullOrEmpty(shpPath) ||
                    (!shpPath.Contains(".shp") && !shpPath.Contains('/'))) return;
                

                if (!File.Exists(shpPath)) return;

                layer = myMap.AddLayer(shpPath);



                layer.CoordinateSystem = CoordSystem.WGS1984;



                layer.Renderer.Clear();

                layer.Visible = isActive;
                layer.Name = mapLayerId;
                layer.Description = lyrName;
                layer.Symbol.Size = 1;


            }
            catch
            {
                return;
            }
        }






        //============= end ===========//

        /*

        if (shpPath.ToUpper().Contains(".SHP") || shpPath.Contains('/'))
        {
            try
            {
                layer = myMap.AddLayer(shpPath);

            }
            catch
            {
                return null;
            }
        }
        else
        {
            try
            {
                using (DataTable dtMap = dbClsAcc.GetMapDataTable(shpPath))
                {
                    //layer = myMap.AddPointDataLayer(dtMap, "LONGITUDE", "LATITUDE");
                    layer = myMap.AddPointDataLayer(dtMap, "LONG", "LAT");
                }
            }
            catch
            {
                return null;
            }
        }

        if (layer == null) return null;

        */

        try
        {
           
            layer.CoordinateSystem = CoordSystem.WGS1984;

            layer.Name = mapLayerId;
            layer.Description = lyrName;

        }
        catch
        {
        }


       

        if (cbLabel.Checked)
        {
            layer.LabelField = lblField;
            layer.LabelFont.Color = Color.FromArgb(48, 23, 24);
            layer.DuplicateLabels = false;
            layer.ShowLabels = true;
        }
        else
        {
            layer.LabelField = "";
            layer.ShowLabels = false;
        }

        
        myMap.CenterAt(myMap.CoordinateSystem.FromWgs84(90, 23.6));


    }

    private void SetMapColorDec(AspMap.Layer layer, string mapSrcPath, string fldName, string strLineColor, int objSize, bool isBasicAdmin, string mapLayerId, bool lgdFlag)
    {
        

        string layType = layer.LayerType.ToString();

        if (layType == "Image")
        {

        }

        string[] colorFlagArray = new string[500];
      
        Color[] myArgbColor = new Color[1000];

        Color[] myArgblineColor = new Color[1000];

        string[] LegendArray = new string[1000];
        int i = 0;
        int LegendIdx = 0;
        int nIndex = 1;


        AspMap.Web.Legend legend1 = new AspMap.Web.Legend();
        legend1.ID = "legend" + (layer.Name).ToString();

        AspMap.FeatureRenderer Renderer = layer.Renderer;
        AspMap.Feature Feature;

        oRs = layer.Recordset;


        //============== start==============//
        string fieldValue = "";
        string fieldName = "";

        string redValue = "";
        string greenValue = "";
        string blueValue = "";

        string lredValue = "";
        string lgreenValue = "";
        string lblueValue = "";

        string symbolSize = "";
        string singleColor = "";

        string lineSize = "";
        string fillColorTag = "";


        string[] featureFieldValue = new string[500];
        string[] featureFieldName = new string[500];

        //============== end ==============//



       // string maplegID = dbClsAcc.GetDataString("SELECT TBLDATAINFO.LEGENDCLASSID FROM    TBLDATAINFO INNER JOIN TBLLEGENDCOLOR ON (TBLDATAINFO.LEGENDCLASSID = TBLLEGENDCOLOR.LEGENDCLASSID) WHERE PRJDATAID= '" + mapLayerId + "'");


        // if (fldName.Length != 0)
        {

               /*

            int counterInt = dbClsAcc.GetRecordCount("SELECT LEGENDCLASSID,SERIALNO, LEGENDNAME,LRED,LGREEN,LBLUE,FRED,FGREEN,FBLUE,SYMBOLSIZE FROM TBLLEGENDCOLOR  WHERE LEGENDCLASSID=" + maplegID);




            if (counterInt > 1)
            {


                using (DataTable lgdInfo = dbClsAcc.GetDataTablesql("SELECT LEGENDCLASSID,SERIALNO, LEGENDNAME,LRED,LGREEN,LBLUE,FRED,FGREEN,FBLUE,SYMBOLSIZE FROM TBLLEGENDCOLOR  WHERE LEGENDCLASSID=" + maplegID + " ORDER BY SERIALNO"))

                {

                    layer.Symbol.LineStyle = LineStyle.Invisible;

                    i = 0;

                    foreach (DataRow readeru in lgdInfo.Rows)
                    {

                        try
                        {

                          

                            fieldName = readeru["LEGENDNAME"].ToString();
                            fieldValue = readeru["LEGENDNAME"].ToString();


                            redValue = readeru["FRED"].ToString();
                            greenValue = readeru["FGREEN"].ToString();
                            blueValue = readeru["FBLUE"].ToString();

                            lredValue = readeru["LRED"].ToString();
                            lgreenValue = readeru["LGREEN"].ToString();
                            lblueValue = readeru["LBLUE"].ToString();

                            // singleColor = readeru["symbolno"].ToString();

                            lineSize = readeru["SYMBOLSIZE"].ToString();



                            string fieldValueStr = fieldValue.ToString();
                            string fieldNameStr = fieldName.ToString();

                            featureFieldValue[i] = fieldValueStr;
                            featureFieldName[i] = fieldNameStr;

                            // featureFieldName[i] = "'"+fieldNameStr+"'";

                            string redValuestr = redValue.ToString();
                            int redValueInt = (redValuestr != "") ? int.Parse(redValuestr) : 0;
                            string greenValueStr = greenValue.ToString();
                            int greenValueInt = (greenValueStr != "") ? int.Parse(greenValueStr) : 0;
                            string blueValueStr = blueValue.ToString();
                            int blueValueInt = (blueValueStr != "") ? int.Parse(blueValueStr) : 0;


                            string lredValuestr = lredValue.ToString();
                            int lredValueInt = (lredValuestr != "") ? int.Parse(lredValuestr) : 0;
                            string lgreenValueStr = lgreenValue.ToString();
                            int lgreenValueInt = (lgreenValueStr != "") ? int.Parse(lgreenValueStr) : 0;
                            string lblueValueStr = lblueValue.ToString();
                            int lblueValueInt = (lblueValueStr != "") ? int.Parse(lblueValueStr) : 0;



                            string symbolSizeStr = symbolSize.ToString();

                            string lineSizeStr = lineSize.ToString();

                            int lineSizeInt = (lineSizeStr != "") ? int.Parse(lineSizeStr) : 1;


                            Renderer.Field = fldName;
                            Feature = Renderer.Add();
                            Feature.Value = fieldValueStr;

                            string ltype = layer.LayerType.ToString();
                            if ((ltype == "Polygon") || (ltype == "Line"))
                            {
                                //  Feature.Symbol.Size = 1;
                            }


                            myArgbColor[i] = Color.FromArgb(redValueInt, greenValueInt, blueValueInt);

                            myArgblineColor[i] = Color.FromArgb(lredValueInt, lgreenValueInt, lblueValueInt);


                            if (ltype == "Line")
                            {

                                layer.Renderer.Clear();
                                layer.Symbol.LineColor = Color.FromArgb(lredValueInt, lgreenValueInt, lblueValueInt);

                                layer.Symbol.Size = lineSizeInt;

                            }
                            else if (ltype == "Polygon" || ltype == "Point")
                            {

                                Feature.Symbol.FillColor = myArgbColor[i];


                                //============ outline false====================//

                                Feature.Symbol.LineStyle = LineStyle.Invisible;
                                Feature.Symbol.LineColor = Color.Transparent;
                                Feature.Symbol.FillStyle = FillStyle.Solid;

                            }

                            legend1.Add(featureFieldName[i], layer.LayerType, Feature.Symbol);





                        }
                        catch (Exception ex)
                        {
                            string exstr = ex.ToString();
                        }


                        i++;

                    }


                }


            }*/
           // else if (counterInt == 0)
            {
                try
                {
                    while (oRs.EOF.Equals(false))
                    {
                        object FeatureName = oRs[fldName];
                        string strFeature = FeatureName.ToString();
                        if (Array.BinarySearch(LegendArray, 0, LegendIdx, strFeature) < 0)
                        {
                            LegendArray[LegendIdx] = strFeature;
                            LegendIdx = LegendIdx + 1;
                            Array.Sort(LegendArray, 0, LegendIdx);
                        }
                        oRs.MoveNext();
                        i++;
                    }


                    for (int j = 0; j < LegendIdx; j++)
                    {
                        Renderer.Field = fldName;
                        Feature = Renderer.Add();
                        Feature.Value = LegendArray[j];

                        string ltype = layer.LayerType.ToString();
                        if ((ltype == "Polygon") || (ltype == "Line"))
                        {
                            Feature.Symbol.Size = 1;
                        }
                        else if (ltype == "Point")
                        {
                            Feature.Symbol.Size = 7;
                        }

                        if (ltype == "Line")
                        {
                            Feature.Symbol.LineColor = GenerateDynamicColor(nIndex);
                        }
                        else if (ltype == "Polygon" || ltype == "Point")
                        {

                            Feature.Symbol.FillColor = GenerateDynamicColor(nIndex);
                            Feature.Symbol.LineColor = Color.FromArgb(102, 102, 102);

                            //============ outline false====================//
                            Feature.Symbol.LineStyle = LineStyle.Invisible;
                            Feature.Symbol.LineColor = Color.Transparent;

                            Feature.Symbol.FillStyle = FillStyle.Solid;

                            //=======================//


                        }


                        nIndex = nIndex + 1;

                        if (cbLegend.Checked)
                        {
                            legend1.Add(LegendArray[j], layer.LayerType, Feature.Symbol);

                        }

                    }


                }
                catch (Exception ex)
                {
                    string exStr = ex.ToString();
                }

            }

            

          //  if (cbLegend.Checked)
            {
                TableRow rw3 = new TableRow();
                TableCell cel3 = new TableCell();
                cel3.Text = "";
                rw3.Cells.Add(cel3);
                TableCell cel4 = new TableCell();
                cel4.HorizontalAlign = HorizontalAlign.Left;
                cel4.Controls.Add(legend1);
                rw3.Cells.Add(cel4);
                bool found = false;
                int z;


                tblLayers.Rows.Add(rw3);
                
            }

            
        }


    }



    static Color GenerateDynamicColor(int currentIndex)
    {
        Color[] colors = {   Color.FromArgb(205, 100, 50),
                                 Color.FromArgb(165, 211, 148),
                                 Color.FromArgb(231, 166, 165),
                                 Color.FromArgb(206, 166, 206),
                                 Color.FromArgb(255, 166, 165),
                                 Color.FromArgb(173, 174, 214),
                                 Color.FromArgb(206, 219, 156),
                                 Color.FromArgb(147, 199, 222),
                                 Color.FromArgb(239, 227, 90),
                                 Color.FromArgb(239, 239, 239),
                                 Color.FromArgb(198, 170, 123),
                                 Color.FromArgb(231, 227, 123),
                                 Color.FromArgb(181, 227, 255),
                                 Color.FromArgb(239, 223, 255),
                                 Color.FromArgb(231, 255, 239),
                                 Color.FromArgb(255, 247, 132),
                                 Color.FromArgb(206, 255, 132),
                                 Color.FromArgb(140, 255, 151),
                                 Color.FromArgb(214, 211, 214),
                                 Color.FromArgb(222, 199, 173),
                                 Color.FromArgb(173, 174, 214) };
        int colorIndex;
        if (currentIndex >= colors.Length - 1)
            colorIndex = currentIndex - (colors.Length - 1) * (int)(currentIndex / (colors.Length - 1));
        else
            colorIndex = currentIndex;
        return colors[colorIndex];
    }

    protected void cbLabel_CheckedChanged(object sender, EventArgs e)
    {

        if (this.cbLabel.Checked == true)
        {
            showLabel();
        }
        else
        {
            hideLabel();
        }

    }


    public void showLabel()
    {
     
        foreach (AspMap.Layer layer in myMap)
        {
            layer.ShowLabels = false;
        }
      

        DataTable mapLayears = dtMapLayers;
        
        string lblName = "";
        string layerName = "";

        try
        {
            DataTable mapLayerstrue = dtMapLayers.Select("LayerActiveMode=TRUE").CopyToDataTable();

            var lastRow = mapLayerstrue.Rows[mapLayerstrue.Rows.Count - 1];


            lblName = lastRow["LabelField"].ToString();
            layerName = lastRow["LayerName"].ToString();

            AspMap.Layer activeLayer = myMap.FindLayer(layerName);

            activeLayer.LabelField = lblName;

            activeLayer.ShowLabels = true;


        }
        catch
        {
        }

      
        

    }


    public void hideLabel()
    {

        foreach (AspMap.Layer layer in myMap)
        {
            layer.ShowLabels = false;
        }

        //DataTable mapLayears = dtMapLayers;



        //int i = 0;
        //foreach (AspMap.Layer layer in myMap)
        //{
        //    i++;

        //}


    }



    private void SetMapColor(AspMap.Layer layer, string fldName)
    {
        AspMap.Feature feature;
        AspMap.FeatureRenderer renderer;
        int LegendIdx = 0;
        int nIndex = 1;
        int i = 0;
        string[] LegendArray = new string[50000];



        if (layer == null || string.IsNullOrEmpty(fldName.Trim()))
            return;

        renderer = layer.Renderer;
        renderer.Field = fldName;

        feature = renderer.Add();
        feature.Value = fldName;

        string lyType = layer.LayerType.ToString();



        if (fldName.ToUpper().Contains("RIV") || ddlDataType.SelectedItem.ToString().ToUpper().Contains("RIV"))
        {
            feature.Symbol.FillColor = Color.FromArgb(13, 24, 245);
            feature.Symbol.LineColor = Color.FromArgb(07, 13, 230);


        }

        

        AspMap.Web.Legend legend1 = new AspMap.Web.Legend();
        legend1.ID = "legend" + (layer.Name).ToString();

        AspMap.FeatureRenderer Renderer = layer.Renderer;

      //  if (fldName.Contains("DIVNAME"))
        {

            oRs = layer.Recordset;

            if (fldName.Length != 0)
            {
                while (oRs.EOF.Equals(false))
                {
                    object FeatureName = oRs[fldName];
                    string strFeature = FeatureName.ToString();

                    if (Array.BinarySearch(LegendArray, 0, LegendIdx, strFeature) < 0)
                    {
                        LegendArray[LegendIdx] = strFeature;
                        LegendIdx = LegendIdx + 1;
                        Array.Sort(LegendArray, 0, LegendIdx);
                    }
                    oRs.MoveNext();
                    i++;
                }

                for (int j = 0; j < LegendIdx; j++)
                {
                    Renderer.Field = fldName;
                    AspMap.Feature Feature = Renderer.Add();
                    Feature.Value = LegendArray[j];

                    string ltype = layer.LayerType.ToString();
                    if ((ltype == "Polygon") || (ltype == "Line"))
                    {
                        Feature.Symbol.Size = 1;
                    }
                    else if (ltype == "Point")
                    {
                        Feature.Symbol.Size = 7;
                    }

                    if (ltype == "Line")
                    {
                        Feature.Symbol.LineColor = GenerateDynamicLineColor(nIndex);
                    }
                    else if (ltype == "Polygon" || ltype == "Point")
                    {
                        Feature.Symbol.FillColor = GenerateDynamicFillColor(nIndex);
                        Feature.Symbol.LineColor = Color.FromArgb(102, 102, 102);
                    }


                    nIndex = nIndex + 1;

                    legend1.Add(LegendArray[j], layer.LayerType, Feature.Symbol);

                    if (cbLegend.Checked)
                    {
                     //   legend1.Add(LegendArray[j], layer.LayerType, Feature.Symbol);

                    }

                }
            }


        }

        


        if (lyType == "Line")
        {
            feature.Symbol.LineColor = Color.FromArgb(new Random().Next(7, 255), new Random().Next(7, 255), new Random().Next(7, 255));
        }
        else if (lyType == "Polygon" || lyType == "Point")
        {
            feature.Symbol.FillColor = Color.FromArgb(new Random().Next(7, 255), new Random().Next(7, 255), new Random().Next(7, 255));
            feature.Symbol.LineColor = Color.FromArgb(175, 75, 78);
        }
        
        if (cbLegend.Checked)
        {
            TableRow rw = new TableRow();
            TableCell cel = new TableCell();
            cel.Text = "";
            rw.Cells.Add(cel);
            TableCell cel1 = new TableCell();
            cel1.Controls.Add(legend1);
            rw.Cells.Add(cel1);

           

            tblLayers.Rows.Add(rw);


        }


    }

    

    protected void AddMapLocator()
    {
        try
        {
            myMapLocator.RemoveAllLayers();
            //if (myMap.LayerCount < 1) return;

            if (!string.IsNullOrEmpty(mapRootDir.Trim()))
                mapRootDir = MapPath(@"ShapeFiles/");

            string filePath = mapRootDir + @"\BD_Locator\Shape\bd_dist.shp";
            

            if (!File.Exists(filePath))
                return;

            Layer layer = myMapLocator.AddLayer(filePath);


            layer.Symbol.LineColor = Color.FromArgb(150, 123, 57);
            layer.Symbol.FillColor = Color.FromArgb(253, 250, 245);
            myMapLocator.ZoomFull();

        }
        catch
        {
        }
    }

   

    private void UpdateMapLocator()
    {
        myMapLocator.ZoomFull();
        myMapLocator.MapShapes.Clear();

        AspMap.Rectangle extent = myMap.Extent;

        // draw the extent of the map as a rectangle
        MapShape mapShape = myMapLocator.MapShapes.Add(extent);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;
        mapShape.Symbol.FillStyle = FillStyle.Invisible;


    }



    protected void ClearAll()
    {
        this.myMap.RemoveAllLayers();
        this.myMapLocator.RemoveAllLayers();
        this.ddlDataType.Items.Clear();
     
    }



    private void UpdateMapInfo()
    {
       // cbLayerList.Items.Clear();

        if (myMap.LayerCount > 0)
        {
            for (int i = 0; i < myMap.LayerCount; i++)
            {
                ListItem item = new ListItem(myMap[i].Name, myMap[i].Name);

                item.Selected = myMap.IsLayerVisible(i);
               // cbLayerList.Items.Add(item);
            }
        }

     
    }


   

    protected void AddNewLayer()
    {


        string mapLayerId = tvDataSource.SelectedNode.Value;

        

        //====rba note-only specific row updating here===== //


        try
        {

            if (string.IsNullOrEmpty(mapLayerId)) return;

            // myMap[mapLayerId].Visible = e.Node.Checked;

            // myMap[mapLayerId].Visible = true;

            using (DataTable dtMapLayersTemp = dtMapLayers)
            {
                for (int l = dtMapLayersTemp.Rows.Count - 1; l >= 0; l--)
                {
                    if (!dtMapLayersTemp.Rows[l]["LayerName"].ToString().Equals(mapLayerId)) continue;

                    dtMapLayersTemp.Rows[l]["LayerActiveMode"] = true;



                  //  mpos = l;
                    dtMapLayersTemp.AcceptChanges();
                    break;
                }

                dtMapLayers = dtMapLayersTemp;
                dtMapLayers.AcceptChanges();
            }
        }
        catch (Exception exception)
        {
            string ex = exception.ToString();
        }


        

    }




    static Color GenerateDynamicFillColor(int currentIndex)
    {
        Color[] colors = {   Color.FromArgb(200, 185, 150), 
                             Color.FromArgb(150, 224, 250),
                             Color.FromArgb(185, 185, 185), 
                             Color.FromArgb(230, 173, 123),
                             Color.FromArgb(200, 235, 145),
                             Color.Pink,
                             Color.RoyalBlue,
                             Color.LawnGreen,
                             Color.MediumOrchid };
        int colorIndex;
        if (currentIndex >= colors.Length - 1)
            colorIndex = currentIndex - (colors.Length - 1) * (int)(currentIndex / (colors.Length - 1));
        else
            colorIndex = currentIndex;
        return colors[colorIndex];
    }



    static Color GenerateDynamicLineColor(int currentIndex)
    {
        Color[] colors = { 
                             Color.FromArgb(100, 73, 17),
                             Color.FromArgb(10, 120, 150),
                             Color.FromArgb(34, 34, 34),
                             Color.FromArgb(150, 73, 17),
                             Color.FromArgb(90, 175, 37),
                             Color.Red,
                             Color.Blue,
                             Color.Green,
                             Color.Maroon
                             };
        int colorIndex;
        if (currentIndex >= colors.Length - 1)
            colorIndex = currentIndex - (colors.Length - 1) * (int)(currentIndex / (colors.Length - 1));
        else
            colorIndex = currentIndex;
        return colors[colorIndex];
    }



    private void GetFieldsAndData(string rstSql, ref object recCount, ref object recArray, ref object fldCount, ref object fldArray, ref object pKeyArray)
    {
       // throw new NotImplementedException();
    }

    #region Default Classes

   
    protected void zoomFull_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        myMap.CenterAt(myMap.CoordinateSystem.FromWgs84(90, 23.6));
        myMap.ZoomLevel = 7;
        

    }

    protected void map_InfoTool(object sender, AspMap.Web.InfoToolEventArgs e)
    {
        /*
        myMap.Callouts.Clear();

        AspMap.Recordset records = myMap.Identify(e.InfoPoint, 5);

        if (!records.EOF)
        {
            dataGrid.DataSource = records;
            dataGrid.DataBind();
            dataGrid.Caption = records.Layer.Name.ToUpper();

            Callout callout = myMap.Callouts.Add();
            callout.X = e.InfoPoint.X;
            callout.Y = e.InfoPoint.Y;
            callout.Text = GetCalloutText(records);
            callout.Font.Size = 16;
        }
       */

    }

    protected String GetCalloutText(AspMap.Recordset rs)
    {
       
        int index = rs.Fields.GetFieldIndex("NAME");
        if (index < 0)
        {
            try
            {
                index = rs.Fields.GetFieldIndex(rs.Layer.LabelField);
            }
            catch
            {
                index = 0;
            }
        }
        if (index < 0) index = 0;
        return (rs.Count > 0 && rs[index] != null) ? rs[index].ToString() : "";



    }

    protected void map_InfoWindowTool(object sender, InfoWindowToolEventArgs e)
    {
        AspMap.Recordset records = myMap.Identify(e.InfoPoint, 5);

       
        e.InfoWindow.HorizontalAlign = HorizontalAlign.Center;
        e.InfoWindow.ScrollBars = System.Web.UI.WebControls.ScrollBars.Auto;
        e.InfoWindow.Content = GetCalloutText(records);
        e.InfoWindow.HorizontalAlign = HorizontalAlign.Center;
      
        if (!records.EOF)
        {
            DataGrid grid = new DataGrid { DataSource = records };
            grid.DataBind();

            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#056b7c");
            grid.BackColor = System.Drawing.Color.AliceBlue;

            e.InfoWindow.Controls.Add(grid);
        }
    }

    protected void map_PointTool(object sender, AspMap.Web.PointToolEventArgs e)
    {
        /*
        myMap.MapShapes.Clear();

        MapShape mapShape = myMap.MapShapes.Add(e.Point);
        mapShape.Symbol.Size = 6;
        mapShape.Symbol.FillColor = Color.Red;

        if (myMap.LayerCount > 0)
        {
            AspMap.Layer activeLayer = myMap[myMap.LayerCount - 1];

            if (activeLayer != null && activeLayer.LayerType == LayerType.Polygon)
            {
                try
                {
                    AspMap.Recordset records = activeLayer.SearchShape(e.Point, SearchMethod.PointInPolygon);

                    if (!records.EOF)
                    {
                        dataGrid.DataSource = records;
                        dataGrid.DataBind();
                        dataGrid.Caption = records.Layer.Name.ToUpper();
                    }
                }
                catch { }
            }
        }

        */

    }

    protected void map_LineTool(object sender, AspMap.Web.LineToolEventArgs e)
    {

        /*
        myMap.MapShapes.Clear();
        MapShape mapShape = myMap.MapShapes.Add(e.Line);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;

        if (myMap.LayerCount > 0)
        {
            AspMap.Layer activeLayer = myMap[myMap.LayerCount - 1];

            if (activeLayer != null)
            {
                try
                {
                    AspMap.Recordset records = activeLayer.SearchShape(e.Line, SearchMethod.Intersect);

                    if (!records.EOF)
                    {
                        dataGrid.DataSource = records;
                        dataGrid.DataBind();
                        dataGrid.Caption = records.Layer.Name.ToUpper();
                    }
                }
                catch { }
            }
        }

       */

    }

    protected void map_PolylineTool(object sender, AspMap.Web.PolylineToolEventArgs e)
    {

        /*
        myMap.MapShapes.Clear();

        MapShape mapShape = myMap.MapShapes.Add(e.Polyline);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;

        if (myMap.LayerCount > 0)
        {
            AspMap.Layer activeLayer = myMap[myMap.LayerCount - 1];

            if (activeLayer != null)
            {
                try
                {
                    AspMap.Recordset records = activeLayer.SearchShape(e.Polyline, SearchMethod.Intersect);

                    if (!records.EOF)
                    {
                        dataGrid.DataSource = records;
                        dataGrid.DataBind();
                        dataGrid.Caption = records.Layer.Name.ToUpper();
                    }
                }
                catch { }
            }
        }

       */


    }

    protected void map_RectangleTool(object sender, AspMap.Web.RectangleToolEventArgs e)
    {
        /*
        myMap.MapShapes.Clear();
        MapShape mapShape = myMap.MapShapes.Add(e.Rectangle);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;
        mapShape.Symbol.FillStyle = FillStyle.Invisible;
        
        if (myMap.LayerCount > 0)
        {
            AspMap.Layer activeLayer = myMap[myMap.LayerCount - 1];

            if (activeLayer != null)
            {
                try
                {
                    AspMap.Recordset records = activeLayer.SearchShape(e.Rectangle, SearchMethod.Inside);

                    if (!records.EOF)
                    {
                        dataGrid.DataSource = records;
                        dataGrid.DataBind();
                        dataGrid.Caption = records.Layer.Name.ToUpper();
                    }
                }
                catch { }
            }
        }

      */

    }

    protected void map_CircleTool(object sender, AspMap.Web.CircleToolEventArgs e)
    {

        /*
        myMap.MapShapes.Clear();

        MapShape mapShape = myMap.MapShapes.Add(e.Circle);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;
        mapShape.Symbol.FillStyle = FillStyle.Invisible;

        if (myMap.LayerCount > 0)
        {
            AspMap.Layer activeLayer = myMap[myMap.LayerCount - 1];

            if (activeLayer != null)
            {
                try
                {
                    AspMap.Recordset records = activeLayer.SearchShape(e.Circle, SearchMethod.Inside);

                    if (!records.EOF)
                    {
                        dataGrid.DataSource = records;
                        dataGrid.DataBind();
                        dataGrid.Caption = records.Layer.Name.ToUpper();
                    }
                }
                catch { }
            }
        }

      */




    }

    protected void map_PolygonTool(object sender, AspMap.Web.PolygonToolEventArgs e)
    {
        /*
        myMap.MapShapes.Clear();

        MapShape mapShape = myMap.MapShapes.Add(e.Polygon);
        mapShape.Symbol.Size = 2;
        mapShape.Symbol.LineColor = Color.Red;
        mapShape.Symbol.FillStyle = FillStyle.Invisible;

        if (myMap.LayerCount > 0)
        {
            AspMap.Layer activeLayer = myMap[myMap.LayerCount - 1];

            if (activeLayer != null)
            {
                try
                {
                    AspMap.Recordset records = activeLayer.SearchShape(e.Polygon, SearchMethod.Inside);

                    if (!records.EOF)
                    {
                        dataGrid.DataSource = records;
                        dataGrid.DataBind();
                        dataGrid.Caption = records.Layer.Name.ToUpper();
                    }
                }
                catch { }
            }
        }


        */

    }

    protected void mapLocator_PointTool(object sender, PointToolEventArgs e)
    {
        AspMap.Rectangle ext = myMap.Extent;
        ext.Offset(e.Point.X - ext.Center.X, e.Point.Y - ext.Center.Y);
        myMap.Extent = ext;
    }
    
#endregion

    protected void btnMapClear_Click(object sender, EventArgs e)
    {
          //http://localhost:58938/DECCMA/DataExplorerN.aspx
        Response.Redirect("DataExplorerN.aspx");

        /*
        cbGoogleMap.Checked = cbLabel.Checked = cbLegend.Checked = false;
        dtMapLayers = null;
        myMap.RemoveAllLayers();
        upMapContainer.Update();

        myMapLocator.RemoveAllLayers();
        upMapLocator.Update();

       // cbLayerList.Items.Clear();
       // upMapInfo.Update();

        if (tvDataSource.SelectedNode != null)
            tvDataSource.SelectedNode.Selected = false;

        upTreeView.Update();

      */



    }



   


   

    private bool IsDataRowExist(DataRow drow)
    {
            if (dtMapLayers.Rows.Count < 1)
                return false;

            var array1 = drow.ItemArray;

            foreach (DataRow dr in dtMapLayers.Rows)
            {           
                var array2 = dr.ItemArray;

                if (array1.SequenceEqual(array2))
                    return true;
            }

            return false;
    }



    private SortDirection SortingDirection
    {
        get
        {
            if (ViewState["SortDirection"] == null)
                ViewState["SortDirection"] = SortDirection.Descending;

            return (SortDirection)ViewState["SortDirection"];
        }
        set
        {
            ViewState["SortDirection"] = value;
        }
    }



    private DataView ObjInfoView
    {
        get { object sObj = Session["_dvObjInfo"]; return (sObj != null) ? (DataView)sObj : null; }
        set { Session["_dvObjInfo"] = value; }
    }
    


    public void tvDataSource_SelectedNodeChanged(object sender, EventArgs e)
    {

        string groupId, typeId, sourceId, dtSource, dsType, dtName, stName;
        string tvSelVal= tvDataSource.SelectedValue;
        string[] words = tvSelVal.Split('_');
        groupId = words[0];
        typeId = words[1];
        sourceId = words[2];
        //======= Set Info for MetaData=========//

        MyDataBase.metMaplayerID = tvSelVal;
        //====================================//

        string np = tvDataSource.SelectedNode.ValuePath.ToString();
        TreeNode node1 = tvDataSource.FindNode(np);
        System.Web.UI.WebControls.TreeNodeCollection nodeList = node1.Parent.ChildNodes;
        int imgC = nodeList.Count;
        tvDataSource.SelectedNode.Select();
        tvDataSource.SelectedNodeStyle.ForeColor = System.Drawing.Color.BlueViolet;
        

       
        dsType = dtName = stName = string.Empty;    

       
        dtSource = this.tvDataSource.SelectedNode.Text;
        MyDataBase.maplayerDes = dtSource;
    



        string dtCount, strCondition;
        string dataType = dbClsAcc.GetDataTypeName(groupId, typeId, sourceId);



     //   MyDataBase.dataType = dataType;


        if (dataType == "Shape")
        {

            //================== start Map Environment=========================//


            /*
            if (myMap.BackgroundLayer == null)
            { 
                AddGoogleMapsLayer();

            if (GoogleMapddl.SelectedValue == "Normal" || GoogleMapddl.SelectedValue == "Physical" ||
                GoogleMapddl.SelectedValue == "Satellite" || GoogleMapddl.SelectedValue == "Hybrid")
            {
                myMap.BackgroundLayer.Visible = true;
            }
            else
            {
                if (myMap.BackgroundLayer != null)
                    myMap.BackgroundLayer.Visible = false;
            }


            //if (GoogleMapddl.SelectedValue == "Normal")
            //{
            //    GoogleMapddl.SelectedValue = "Normal";
            //    myMap.BackgroundLayer.Visible = true;

            //}

            myMap.ImageFormat = ImageFormat.Png;

            }

            */

            //=================== end Map =======================//





            MyDataBase.dataType = dataType;


            multiView.ActiveViewIndex = 0;



            DataRow currentRow, nextRow, temp, lastrow;
            int mpos = 0;



            string mapLayerId = tvDataSource.SelectedNode.Value;

          

            DataRow[] mapDataList = dtMapLayers.Select().Where(dr => dr["LayerName"].Equals(mapLayerId)).ToArray();
            if (!mapDataList.Any())
                return;

            

            try
            {

                if (string.IsNullOrEmpty(mapLayerId)) return;

                // myMap[mapLayerId].Visible = e.Node.Checked;

                myMap[mapLayerId].Visible = true;

                using (DataTable dtMapLayersTemp = dtMapLayers)
                {
                    for (int l = dtMapLayersTemp.Rows.Count - 1; l >= 0; l--)
                    {
                        if (!dtMapLayersTemp.Rows[l]["LayerName"].ToString().Equals(mapLayerId)) continue;

                        dtMapLayersTemp.Rows[l]["LayerActiveMode"] = true;



                        mpos = l;

                        dtMapLayersTemp.AcceptChanges();
                        break;
                    }

                    dtMapLayers = dtMapLayersTemp;
                    dtMapLayers.AcceptChanges();
                }
            }
            catch (Exception exception)
            {
                string ex = exception.ToString();
            }

            

            currentRow = dtMapLayers.Rows[mpos];

            DataRow oldRow = currentRow;
            DataRow newRow = dtMapLayers.NewRow();

            newRow.ItemArray = oldRow.ItemArray;


            {

                {
                    dtMapLayers.Rows.Remove(oldRow);
                    dtMapLayers.Rows.InsertAt(newRow, dtMapLayers.Rows.Count);


                    dtMapLayers.AcceptChanges();


                }
            }

            

            addLayerName();

            Thread.Sleep(2000);


          //  if (e.Node.Checked == true)
            {

                AspMap.Layer activeLayer = myMap.FindLayer(mapLayerId);
                myMap.RemoveLayer(activeLayer);

                if (string.IsNullOrEmpty(mapRootDir))
                    mapRootDir = MapPath(@"ShapeFiles/");

                var mapLayers = dtMapLayers.Select("LayerName='" + mapLayerId + "' AND LayerActiveMode=TRUE");
               
                

                if (!mapLayers.Any()) return;

                {

                    string shapeSource = mapLayers[0]["ShapeFilePath"].ToString();
                    string layerName = mapLayers[0]["LayerName"].ToString();


                    string layerDescrip = mapLayers[0]["LayerOrgName"].ToString();
                    string filePath = mapRootDir + shapeSource;

                    DataRow[] topLayerdr = mapLayers;

                    AddMapLayerDec(topLayerdr[0]);
                    addLayerName();
                    

                    ShowAttributeTable(mapLayerId);

                }


            }

            

               AddMapLocator();

               upMapLocator.Update();
               UpdateLayerListcbaddl();


            //=========== label start =================//


            if (this.cbLabel.Checked == true)
            {
                showLabel();
            }
            else
            {
                hideLabel();
            }

            

            // tvDataSource.Attributes.Add("onclick", "postBackByObject()");
            

            myMap.ZoomLevel = 7;
            myMap.CenterAt(myMap.CoordinateSystem.FromWgs84(90, 23.6));

           // myMap.Dispose();

        }
        if (dataType== "Tabular" || dataType == "TimeSeries")
        {

          //  tvDataSource.Attributes.Add("onclick", "postBackByObject()");
            // dvAllData.Visible = true;
            MyDataBase.dataType = dataType;

            //===========  page pre render=====================// 


            // if (MyDataBase.dataType == "Tabular" && myMap.BackgroundLayer != null)


            /*
             
            myMap.BackgroundLayer = null;
            myMap.Callouts.Clear();
            myMap.Markers.Clear();
            myMap.RemoveAllLayers();
            myMap.MapShapes.Clear();
             
             */


       
       /*

            if ( myMap.BackgroundLayer != null)
            {
                myMap.BackgroundLayer.Visible = false;


                myMap.Callouts.Clear();
                myMap.Markers.Clear();
               // myMap.RemoveAllLayers();
                myMap.MapShapes.Clear();
                myMap.RemoveAllLayers();

                myMap.BackgroundLayer = null;



            }
            else if ( myMap.BackgroundLayer == null)
                return;



            */



            //if (MyDataBase.dataType == "Tabular" && myMap.BackgroundLayer != null)
            //{
            //    myMap.RemoveAllLayers();

            //    myMap.BackgroundLayer.Visible = false;
            //}
            //else if (MyDataBase.dataType == "Tabular" && myMap.BackgroundLayer == null)

            //    return;




            //============ page pre render===================//



            //  gmap.overlayMapTypes.setAt(0, null);



            //=========== start pre==================//


            /*
            myMap.MapShapes.Clear();
            if (myMap.BackgroundLayer != null)
            {   
              myMap.BackgroundLayer.Visible = false;
                myMap.BackgroundLayer = null;
            } 

            */
            //============ end pre==================//


            //============ start up=================//
            /*
            if (myMap.BackgroundLayer != null)
            {
                myMap.BackgroundLayer.Visible = false;
            }
            else if (myMap.BackgroundLayer == null)
            {
               return;  
            }
            */ 

            //============  end up=================//



            //==================== start =======================//

            /*
            try
            {
                // AddGoogleMapsLayerSensor();
                //============ start==============//
                myMap.BackgroundLayer.Visible = false;

                if (myMap.BackgroundLayer != null)
                {
                    
                    myMap.BackgroundLayer.Visible = false;

                    // myMap.BackgroundLayer = nul
                    // myMap.BackgroundLayer.
                }



            }
            catch (Exception ex)
            {
                if (myMap.BackgroundLayer == null)
                    return;

            }

            */

            //===================== End ======================//




            multiView.ActiveViewIndex = 1; 


          //  multiView.GetActiveView();


         //   MyDataBase.dtg = null;
          //  dvAllData.DataSource = null;
          //  multiView.SetActiveView(tableView);
            string tblName = dbClsAcc.GetDataTableName(groupId, typeId, sourceId);

            // using (DataTable dt = dbClsAcc.GetDataTable(tblName)) 
            // MyDataBase

            //============== start ================//

            datatable.tabDt = dbClsAcc.GetDataTable(tblName);
            //MyDataBase.dtg = dbClsAcc.GetDataTable(tblName);
            dtCount = datatable.tabDt.Rows.Count.ToString();


            dvAllData.DataSource = datatable.tabDt;
            dvAllData.DataBind();

            dvAllData.Visible = true;
           // BindGridView(dvAllData, MyDataBase.dtg);

            //============== end ================//


            /*
            using (MyDataBase.dtg = dbClsAcc.GetDataTable(tblName))
            {
                dtCount = MyDataBase.dtg.Rows.Count.ToString();

                dtCount = BindGridView(dvAllData, MyDataBase.dtg);

            }

            */


            lblMessage.Text = ">> Total Records available : " + dtCount;
           // lblMessage.Visible = true;

            //upDataGrid.Update();
        }
        

           
    }


    private void ShowAttributeTable(string mapLayerId)
    {
        
        string layerName = "";

        try
        {
            DataTable mapLayerstrue = dtMapLayers.Select("LayerActiveMode=TRUE").CopyToDataTable();
            var lastRow = mapLayerstrue.Rows[mapLayerstrue.Rows.Count - 1];
            layerName = lastRow["LayerName"].ToString();
            
        }
        catch
        {
        }
        

        string selMaplayerid = layerName;
        MyDataBase.attMaplayerID = selMaplayerid;

        AspMap.Recordset rs = null;
        
        if (selMaplayerid != "")
        {
            AspMap.Layer activeLayer = myMap.FindLayer(selMaplayerid);
            rs = activeLayer.Recordset;
            myMap.Callouts.Clear();

            Session["key"] = rs;

        }
        else if (selMaplayerid == "")
        {
            Session["key"] = rs;
        }



    }

    protected void AddNewLayerDec()
    {

        DataRow currentRow, nextRow, temp, lastrow;
        int mpos = 0;
        
        string mapLayerId = tvDataSource.SelectedNode.Value;
        
        DataRow[] mapDataList = dtMapLayers.Select().Where(dr => dr["LayerName"].Equals(mapLayerId)).ToArray();
        if (!mapDataList.Any())
            return;

        
        try
        {

            if (string.IsNullOrEmpty(mapLayerId)) return;

            // myMap[mapLayerId].Visible = e.Node.Checked;

            myMap[mapLayerId].Visible = true;

            using (DataTable dtMapLayersTemp = dtMapLayers)
            {
                for (int l = dtMapLayersTemp.Rows.Count - 1; l >= 0; l--)
                {
                    if (!dtMapLayersTemp.Rows[l]["LayerName"].ToString().Equals(mapLayerId)) continue;

                    dtMapLayersTemp.Rows[l]["LayerActiveMode"] = true;
                    

                    mpos = l;

                    dtMapLayersTemp.AcceptChanges();
                    break;
                }

                dtMapLayers = dtMapLayersTemp;
                dtMapLayers.AcceptChanges();
            }
        }
        catch (Exception exception)
        {
            string ex = exception.ToString();
        }


        //========== start ==========/



        currentRow = dtMapLayers.Rows[mpos];

        DataRow oldRow = currentRow;
        DataRow newRow = dtMapLayers.NewRow();

        newRow.ItemArray = oldRow.ItemArray;


        {

            {
                dtMapLayers.Rows.Remove(oldRow);
                dtMapLayers.Rows.InsertAt(newRow, dtMapLayers.Rows.Count);


                dtMapLayers.AcceptChanges();


            }
        }





    }

    //https://stackoverflow.com/questions/12545718/how-to-avoid-duplicate-items-being-added-from-one-checkboxlist-to-another-checkb

    public void UpdateLayerListcbaddl()
    {

        cbLayerList.Items.Clear();

        try
        {

            using (DataTable dtMapLayerscb = dtMapLayers.Select("LayerActiveMode=TRUE").CopyToDataTable())
            {
                for (int l = dtMapLayerscb.Rows.Count - 1; l >= 0; l--)
                {
                    string layerName = dtMapLayerscb.Rows[l]["LayerOrgName"].ToString();
                    string projectId = dtMapLayerscb.Rows[l]["LayerName"].ToString();
                    
                    ListItem Item = new ListItem(layerName, projectId);


                    if (!cbLayerList.Items.Contains(Item))
                    {
                        cbLayerList.Items.Add(new ListItem(layerName, projectId));

                        cbLayerList.DataBind();

                    }
                   

                }

                foreach (ListItem li in cbLayerList.Items)
                {

                        li.Selected = true;
                }
                
            }

            cblayerLstup.Update();

        }
        catch (Exception exception)
        {
            string ex = exception.ToString();
        }

        


    }


    protected void cbLayerList_DataBound(object sender, EventArgs e)
    {
        foreach (ListItem item in cbLayerList.Items)
            item.Attributes.Add("alt", item.Value);
    }

    protected void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
    {
        

        DataTable dtMapLayersTempcb = dtMapLayers;

        foreach (ListItem chkitem in cbLayerList.Items)
        {


            // string maLayerID = chkitem.Value;

            if (chkitem.Selected == true)
            {
                // myMap[chkitem.Value].Visible = chkitem.Selected;


                //============ start =================//
                try
                {

                    // if (string.IsNullOrEmpty(mapLayerId)) return;

                    // myMap[mapLayerId].Visible = e.Node.Checked;

                    // myMap[mapLayerId].Visible = true;

                    using (DataTable dtMapLayersTemp = dtMapLayers)
                    {
                        for (int l = dtMapLayersTemp.Rows.Count - 1; l >= 0; l--)
                        {
                            if (!dtMapLayersTemp.Rows[l]["LayerName"].ToString().Equals(chkitem.Value)) continue;

                            dtMapLayersTemp.Rows[l]["LayerActiveMode"] = true;



                            // mpos = l;
                            dtMapLayersTemp.AcceptChanges();
                            break;
                        }

                        dtMapLayers = dtMapLayersTemp;
                        dtMapLayers.AcceptChanges();
                    }
                }
                catch (Exception exception)
                {
                    string ex = exception.ToString();
                }
                

                myMap[chkitem.Value].Visible = chkitem.Selected;




            }
            else if (chkitem.Selected == false)
            {


                myMap[chkitem.Value].Visible = false;


                try
                {

                    using (DataTable dtMapLayersTemp = dtMapLayers)
                    {
                        for (int l = dtMapLayersTemp.Rows.Count - 1; l >= 0; l--)
                        {
                            if (!dtMapLayersTemp.Rows[l]["LayerName"].ToString().Equals(chkitem.Value)) continue;

                            dtMapLayersTemp.Rows[l]["LayerActiveMode"] = false;



                            // mpos = l;
                            dtMapLayersTemp.AcceptChanges();
                            break;
                        }

                        dtMapLayers = dtMapLayersTemp;
                        dtMapLayers.AcceptChanges();
                    }
                }
                catch (Exception exception)
                {
                    string ex = exception.ToString();
                }




            }



            addLayerName();



        }




        if (this.cbLabel.Checked == true)
        {
            showLabel();
        }
        else
        {
            hideLabel();
        }

        
        DataTable mapLayears = dtMapLayers;
        

        string layDesc= "";
        string layerName = "";

        try
        {
            DataTable mapLayerstrue = dtMapLayers.Select("LayerActiveMode=TRUE").CopyToDataTable();

            var lastRow = mapLayerstrue.Rows[mapLayerstrue.Rows.Count - 1];


            layerName = lastRow["LayerName"].ToString();
            layDesc = lastRow["LayerOrgName"].ToString();

            MyDataBase.metMaplayerID = layerName;

            MyDataBase.maplayerDes = layDesc;

            
        }
        catch
        {
        }

        

    }

    protected void dvAllData_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dvAllData.PageIndex = e.NewPageIndex;
        //  DataViewRefresh();

        // dvAllData.DataSource = ObjInfoView;

        dvAllData.DataSource = datatable.tabDt;

        dvAllData.DataBind();
    }


    //protected void dvAllData_Sorting(object sender, GridViewSortEventArgs e)
    //{
    //    ReSortingInfo(e.SortExpression);
    //}


    protected void Menu1_MenuItemClick(object sender, MenuEventArgs e)
    {
        /*
         MultiView1.ActiveViewIndex = Int32.Parse(e.Item.Value)
        Dim i As Integer
        'Make the selected menu item reflect the correct imageurl
        For i = 0 To Menu1.Items.Count - 1
            If i = e.Item.Value Then
                Menu1.Items(i).ImageUrl = "selectedtab.gif"
            Else
                Menu1.Items(i).ImageUrl = "unselectedtab.gif"
            End If
        Next
         */

        multiView.ActiveViewIndex = Int32.Parse(e.Item.Value);

        //for (int i = 0; i < Menu1.Items.Count - 1; i++)
        //{
        //    if (i==Int32.Parse(e.Item.Value))
        //    {   //http://localhost:63891/Resources/images/selectedtab.GIF
        //       // Menu1.Items(i).ImageUrl = "~/Resources/images/selectedtab.gif";
        //        multiView.ActiveViewIndex = 0;
        //        // Menu1.i
        //    }
        //}


    }


    protected void btnShow_Click(object sender, EventArgs e)
    {
        
    }



    public string BindGridView(GridView myObj, string dtName, string condition)
    {
                string sql = @"SELECT * FROM " + dtName + condition;

                DataTable dt = dbClsAcc.GetDataTable(sql);

                int rc = (dt != null) ? dt.Rows.Count : 0;

                if (rc > 0)
                {
                    myObj.Columns.Clear();
                    myObj.AutoGenerateColumns = true;

                    ObjInfoView = new DataView(dt);
                    SortingDirection = SortDirection.Descending;

                    myObj.DataSource = dt;
                }
                else
                    myObj.DataSource = null;

                myObj.DataBind();

                return rc.ToString();
    }



    public string BindGridView(GridView myObj, DataTable dt)
    {
        int rc = (dt != null) ? dt.Rows.Count : 0;

        if (rc > 0)
        {
            myObj.Columns.Clear();
            myObj.AutoGenerateColumns = false;
            
            foreach (DataColumn dc in dt.Columns)
            {
                BoundField myBoundField = new BoundField();

                myBoundField.DataField = dc.ColumnName;
                myBoundField.HeaderText = dc.ColumnName;
                //myBoundField.SortExpression = dc.ColumnName;

                //if (dc.DataType == typeof(DateTime))
                //{
                //    myBoundField.DataFormatString = "{0:dd-MM-yyyy}";
                //    myBoundField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                //}
                //else if (dc.DataType == typeof(Decimal))
                //{
                //    myBoundField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                //}

                myObj.Columns.Add(myBoundField);
            }
            
          //  ObjInfoView = new DataView(dt);
           // SortingDirection = SortDirection.Descending;

            myObj.DataSource = dt;
            myObj.DataBind(); //rba
        }
        //else
        //    myObj.DataSource = null;

        //    myObj.DataBind();

        return rc.ToString();
    }
    


    //protected void dvAllData_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //  //  dvAllData.PageIndex = e.NewPageIndex;
    //    DataViewRefresh();
    //}



    //private bool DataViewRefresh()
    //{
        

    //    try
    //    {
    //        dvAllData.DataSource = ObjInfoView;
    //        dvAllData.DataBind();
    //        return true;
    //    }
    //    catch
    //    {
    //        dvAllData.DataSource = null;
    //        dvAllData.DataBind();
    //        return false;
    //    }
          

    //    return true;
    //}



    //protected void dvAllData_Sorting(object sender, GridViewSortEventArgs e)
    //{
    //    ReSortingInfo(e.SortExpression);
    //}


    //protected void ReSortingInfo(string sortExpr)
    //{
    //    string sortiOrder = string.Empty;

    //    if (SortingDirection == SortDirection.Descending)
    //    {
    //        SortingDirection = SortDirection.Ascending;
    //        sortiOrder = "Asc";
    //    }
    //    else
    //    {
    //        SortingDirection = SortDirection.Descending;
    //        sortiOrder = "Desc";
    //    }

    //    if (ObjInfoView != null)
    //        ObjInfoView.Sort = sortExpr + " " + sortiOrder;

    //    DataViewRefresh();
    //}



    public string SetCondition(string stationId, string dateFieldName, string startingDate, string endingDate)
    {
        string strCondition = "";

        if (!string.IsNullOrEmpty(stationId))
        {
            strCondition = strCondition + " (STATIONID='" + stationId + "') ";

            if (!string.IsNullOrEmpty(startingDate) && !string.IsNullOrEmpty(endingDate))
            {
                try
                {
                    
                    if (string.IsNullOrEmpty(dateFieldName))
                        dateFieldName = "DATETIME";

                    string datRange = "(" + dateFieldName + " BETWEEN TO_DATE ('" + startingDate + "', 'DD-MM-YYYY') AND TO_DATE ('" + endingDate + "', 'DD-MM-YYYY'))";

                    strCondition += " AND " + datRange;
                   
                }
                catch { }
            }
        }

        if (!string.IsNullOrEmpty(strCondition))
            strCondition = " WHERE " + strCondition;

        return strCondition;
    }


   


   


}