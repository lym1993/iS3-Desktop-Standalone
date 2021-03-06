﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iS3.Core;
using System.Data;
using iS3.Core.Serialization;
using iS3.Geology.Serialization;
using System.Windows;
using iS3.Geology.UserControls;

namespace iS3.Geology
{
    //参照BoreholeGeology
    

    //PileFoundation类，代表桩基础的基本性质
    public class PileFoundation:DGObject
    {
        //ID和Name、Shape继承自DGObject，不用再定义
        
        //桩基础ID、绑定表1的ID字段
        public int PFID { get; set; }
        //桩基础形状
        public string Type { get; set; }
        //矩形承台长边l
        public double L { get; set; }
        //矩形承台短边b
        public double B { get; set; }
        //圆形承台半径
        public double R { get; set; }
        //荷载
        public double Load { get; set; }
        //承台顶标高
        public double TopOfCushionCap { get; set; }
        //桩顶标高OR承台底部标高
        public double TopOfPile { get; set; }
        //桩底部标高
        public double BaseOfPile { get; set; }
       
        //桩基础横坐标
        public double Xcoordinate { get; set; }
        //桩基础纵坐标
        public double Ycoordinate { get; set; }
        //桩长
        public double PileLength { get; set; }
        //承台底桩径
        public double DiameterOfPile { get; set; }
        //承台下总桩数
        public int NumberOfPile { get; set; }
        //矩形桩基础短边桩数量
        public int PileOfB { get; set; }
        //桩间距离
        public double DistanceOfPile { get; set; }
        
        //针对桩基础地质情况集合的列表
        //第二个表中的数据，其中PileFoundationID与表一中的ID是对应的
        //一个Geologies列表包含一个PileFoundation实例的信息
        public List<PileFoundationGeology> Geologies { get; set; }

        //PileFoundation的构造函数,构造新列表赋值给Geologies列表
        public PileFoundation()
        {
            Geologies = new List<PileFoundationGeology>();
        }

        //含database参数的构造函数
        public PileFoundation(DataRow rawData):base(rawData)
        {
            Geologies = new List<PileFoundationGeology>();
        }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            //GDGOLoader中没有定义LoadPileFoundation方法，重新去定义一下
            GeologyDGObjectLoader loader = new GeologyDGObjectLoader(dbContext);
            bool success = loader.LoadPileFoundations(objs);
            return success;
        }
        
        //现在用不上
        //ToString覆写方法
        public override string ToString()
        {
            string str = base.ToString();

            //桩底标高、桩顶标高，基础形状
            string str1 = string.Format(
                ", TopOfPile={0}, BaseOfPile={1},  Type={2}, Geo=",
                TopOfPile, BaseOfPile,  Type);
            str += str1;

            foreach (var geo in Geologies)
            {
                str += geo.StratumID + ",";
            }

            return str;
        }

        //200311参照Borehole格式写表格视图
        public override List<DataView> tableViews(IEnumerable<DGObject> objs)
        {
            List<DataView> dataViews = base.tableViews(objs);

            if (parent.rawDataSet.Tables.Count > 1)
            {
                DataTable table = parent.rawDataSet.Tables[1];
                string filter = idFilter(objs);
                DataView view = new DataView(table, filter, "[PileFoundationID]",
                    DataViewRowState.CurrentRows);
                dataViews.Add(view);
            }

            return dataViews;
        }

        //字符串加工
        //没什么用
        string idFilter(IEnumerable<DGObject> objs)
        {
            string sql = "PileFoundationID in (";
            foreach (var obj in objs)
            {
                sql += obj.ID.ToString();
                sql += ",";
            }
            sql += ")";
            return sql;
        }

        //这是wpf显示控件图表视图
        //如果需要右下角显示图像，改这里
        public override List<FrameworkElement> chartViews(
            IEnumerable<DGObject> objs, double width, double height)
        {
            List<FrameworkElement> charts = new List<FrameworkElement>();

            List<PileFoundation> pfs = new List<PileFoundation>();
            foreach (PileFoundation pf in objs)
            {
                if (pf != null && pf.Geologies.Count > 0)
                    pfs.Add(pf);
            }
            //200313.Geology改为Construnction
            Domain geologyDomain = Globals.project.getDomain(DomainType.Construction);
            
            //DGObjectsCollection strata = geologyDomain.getObjects("Stratum");


            //实例化新的钻孔显示界面，这里尚未改动
            PileFoundationCollectionView pfsView = new PileFoundationCollectionView();
            pfsView.Name = "Geology";
            pfsView.PileFoundations = pfs;
            //地层数据没有绑定上
            pfsView.Strata = null;
            pfsView.ViewerHeight = height;
            pfsView.RefreshView();
            pfsView.UpdateLayout();
            charts.Add(pfsView);

            return charts;
        }

        //20200320
        //沉降计算模块，不知道能不能用
        public void PileFoundationCalculate()
        {
            MessageBox.Show("桩基计算模块开始！");

            #region 桩基础扩展方法
            
            //option为0时，采用默认得odbc读取方法
            //option为1时，采用oledb方法读取
            //string definitionFile = "PileFoundationTest.xml";
            //DbContext dbContext = new DbContext("Data\\PileFoundationTest\\PileFoundationTest.mdb", 0);
            DbContext dbContext = new DbContext("Data\\Z14\\Z14.mdb", 0);
            //定义DGObjectsDefinition的各项属性
            DGObjectsDefinition def = new DGObjectsDefinition();
            def.DefNamesSQL = null;
            def.Name = "AllPileFoundations";
            def.Type = "PileFoundation";
            def.TableNameSQL = "PileFoundation,PileFoundationStrataInfo";
            def.OrderSQL = "ID,ID";

            //Load方法
            //实例化objs
            DGObjects objs = new DGObjects(def);
            bool success = objs.load(dbContext);

            DGObjects objcal = objs;

            foreach (var pf in objs.values)
            {
                
                MessageBox.Show("转换成功！");
            }

            //objs的rawdataset属性
            objs.rawDataSet = new System.Data.DataSet();

            DGObject objhelper = ObjectHelper.CreateDGObjectFromSubclassName(def.Type);
            objhelper.LoadObjs(objs, dbContext);
            objs.buildIDIndex();
            objs.buildRowViewIndex();

            MessageBox.Show("该类调用完成！");
            //objscal.count是桩基础的数量，进行循环
            #endregion
        }
    }

    //代表桩基础的地质性质（各层标高、天然地层等等）表2信息
    public class PileFoundationGeology
    {
        //200319新加PFID
        public long PileFoundationID { get; set; }
        //地层顶标高
        public double Top { get; set; }
        //地层底标高
        public double Base { get; set; }
        //天然地层编号
        public string StratumID { get; set; }
        //土壤重度gama
        public double Gama { get; set; }
        //土壤压缩模量Es
        public double Es0_100 { get; set; }
        public double Es100_200 { get; set; }
        public double Es200_300 { get; set; }
        public double Es300_400 { get; set; }
        public double Es400_500 { get; set; }

    }

    
}
