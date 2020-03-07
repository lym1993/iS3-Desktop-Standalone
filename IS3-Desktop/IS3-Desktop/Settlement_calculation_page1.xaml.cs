﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iS3.Core;
using iS3.Core.Serialization;
using iS3.Geology;
using iS3.Geology.Serialization;

namespace iS3.Desktop
{
    /// <summary>
    /// Settlement_calculation_page1.xaml 的交互逻辑
    /// </summary>
    public partial class Settlement_calculation_page1 : Window
    {
        public Settlement_calculation_page1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //点击计算按钮后，开始读取access表中数据
            //运用到的两个类DbContext和GeologyDGObjectLoader
            //因为仅仅使用database（后面画等值线需要GISLayer?）
            //option为0时，采用默认得odbc读取方法
            //option为1时，采用oledb方法读取
            //先设置环境
            
            DbContext dbContext = new DbContext("Data\\PileFoundationTest\\PileFoundationTest.mdb", 0);

            //定义DGObjectsDefinition的各项属性
            DGObjectsDefinition dGObjectsDefinition = new DGObjectsDefinition();
            dGObjectsDefinition.DefNamesSQL = null;
            dGObjectsDefinition.Name = "AllPileFoundations";
            dGObjectsDefinition.Type = "PileFoundation";
            dGObjectsDefinition.TableNameSQL = "PileFoundation,PileFoundationStrataInfo";
            dGObjectsDefinition.OrderSQL = "Name";

            DGObjects objs = new DGObjects(dGObjectsDefinition);
            //objs的rawdataset属性
            objs.rawDataSet = new System.Data.DataSet();

            GeologyDbDataLoader geologyDbDataLoader = new GeologyDbDataLoader(dbContext);
            //新增readpilefoundationinformation方法
            geologyDbDataLoader.ReadPileFoundationInformation( objs, "PileFoundation,PileFoundationStrataInfo",""
            , "Name");

            MessageBox.Show("shuijiao");
            
            
            

            
            
            
            


        }
    }
}
