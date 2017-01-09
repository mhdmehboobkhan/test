using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel;
using TestApp.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Tesseract;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        DataBaseAccess db = new DataBaseAccess();

        public ActionResult Index()
        {
            //var list = db.Get_DT_SP("Select * From [User]", CommandType.Text);
            //var foo2 = list.Cast<IDataRecord>().Select(dr => new user
            //{
            //    id = int.Parse(dr["id"].ToString()),
            //    name = dr["name"] != null ? dr["name"].ToString() : "",
            //    email = dr["email"] != null ? dr["email"].ToString() : "",
            //    pass = dr["pass"] != null ? dr["pass"].ToString() : ""
            //}).ToList();

            //List<user> list3 = list.Select(x => new user
            //{
            //    name = x.name
            //}).ToList();
            //List<user> list2 = list.Cast<user>().ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Index(Customer model)
        {
            List<Customer> List = new List<Customer>();
            var path = "";

            try
            {
                var FileName = Request.Files["importexcelfile"];
                FileName.SaveAs(Server.MapPath(Path.Combine("~/Content/ExcelFile/", Path.GetFileName(FileName.FileName))));
                path = Server.MapPath(@"~\Content\ExcelFile\" + Path.GetFileName(FileName.FileName));

                foreach (var worksheet in Workbook.Worksheets(path))
                {
                    foreach (var row in worksheet.Rows.Skip(1))
                    {
                        if (row != null)
                        {
                            model = new Customer();
                            model.CustomerGuid = Guid.NewGuid();
                            if (row.Cells[0] != null)
                            {
                                model.email = row.Cells[0].Text;
                            }
                            if (row.Cells[3] != null)
                            {
                                model.password_hash = row.Cells[3].Text;
                            }

                            if (row.Cells[4] != null || row.Cells[1] != null)
                            {
                                if (row.Cells[4] != null)
                                    model.CustomerAddress._address_firstname = row.Cells[4].Text;
                                else if (row.Cells[1] != null)
                                    model.CustomerAddress._address_firstname = row.Cells[1].Text;
                            }
                            if (row.Cells[5] != null || row.Cells[2] != null)
                            {
                                if (row.Cells[5] != null)
                                    model.CustomerAddress._address_lastname = row.Cells[5].Text;
                                else if (row.Cells[2] != null)
                                    model.CustomerAddress._address_lastname = row.Cells[2].Text;
                            }
                            if (row.Cells[6] != null)
                            {
                                model.CustomerAddress._address_Company = row.Cells[6].Text;
                            }
                            if (row.Cells[7] != null)
                            {
                                model.CustomerAddress._address_country_id = row.Cells[7].Text;
                            }
                            if (row.Cells[8] != null)
                            {
                                model.CustomerAddress._address_city = row.Cells[8].Text;
                            }
                            if (row.Cells[9] != null)
                            {
                                model.CustomerAddress._address_street = row.Cells[9].Text;
                            }
                            if (row.Cells[10] != null)
                            {
                                model.CustomerAddress._address_postcode = row.Cells[10].Text.Replace(",","");
                            }
                            if (row.Cells[11] != null)
                            {
                                model.CustomerAddress._address_telephone = row.Cells[11].Text.Replace(",", "");
                            }
                            if (row.Cells[12] != null)
                            {
                                model.CustomerAddress._address_fax = row.Cells[12].Text;
                            }

                            List.Add(model);
                        }
                    }

                    if (List.Count() > 0)
                        InsertIntoDataBase(List);

                    break;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException;
            }
            if(path != "")
                System.IO.File.Delete(path);
            return View();
        }

        public void InsertIntoDataBase(List<Customer> List)
        {
            DataBaseAccess dbConnection = new DataBaseAccess();
            SqlTransaction objTrans = null;
            using (SqlConnection objConn = new SqlConnection(DataBaseAccess.m_ConnectionString))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    foreach(var lis in List)
                    {
                        SqlParameter[] cmdRecordsListTr = {
                                    new SqlParameter("@CustomerGuid", SqlDbType.UniqueIdentifier, 16, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerGuid),
                                    new SqlParameter("@email", SqlDbType.NVarChar, 1000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.email),
                                    new SqlParameter("@password_hash", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.password_hash),
                                    new SqlParameter("@_address_firstname", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_firstname),
                                    new SqlParameter("@_address_lastname", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_lastname),
                                    new SqlParameter("@_address_Company", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_Company),
                                    new SqlParameter("@_address_country_id", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_country_id),
                                    new SqlParameter("@_address_city", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_city),
                                    new SqlParameter("@_address_street", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_street),
                                    new SqlParameter("@_address_postcode", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_postcode),
                                    new SqlParameter("@_address_telephone", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_telephone),
                                    new SqlParameter("@_address_fax", SqlDbType.NVarChar, -1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, lis.CustomerAddress._address_fax)
                                };

                        dbConnection.GetDataSet_BySQLTransactions(new SqlCommand("sp_InsertCustomer", objConn, objTrans), cmdRecordsListTr);
                    }

                    objTrans.Commit();
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                }
                finally
                {
                    objConn.Close();
                }
               
            }
        }


        public ActionResult About()
        {
            var testImagePath = Server.MapPath(@"~\Content\images\TestImage2.jpg");
            var dataPath = @"D:\Sample Projects\TestApp\tessdata";

            try
            {
                using (var tEngine = new TesseractEngine(dataPath, "eng", EngineMode.Default)) //creating the tesseract OCR engine with English as the language
                {
                    using (var img = Pix.LoadFromFile(testImagePath)) // Load of the image file from the Pix object which is a wrapper for Leptonica PIX structure
                    {
                        using (var page = tEngine.Process(img)) //process the specified image
                        {
                            var text = page.GetText(); //Gets the image's content as plain text.
                            ViewBag.Message = page.GetMeanConfidence();
                            //Console.WriteLine(text); //display the text
                            //Console.WriteLine(page.GetMeanConfidence()); //Get's the mean confidence that as a percentage of the recognized text.
                            //Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error: " + e.Message);
            }

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {

            GetNotfoundSKUs();
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string GetNotfoundSKUs()
        {
            string fileStrings = "CWCB-16V3-EAP#01-Pearlesque WH,CWCB-16V3-EAP#01-Pearlesque WH,CWCB-16V3-EAP#01-Pearlesque WH,CWCB-16V3-EAP#03-Shade BLACK S,CWCB-16V3-EAP#03-Shade BLACK L,CWCB-16V2-EBP#01-Black Pine BL,CWJB-16V3-JAP#01-Sand Dune OFF,CWJB-16V3-JAP#01-Sand Dune OFF,CWJB-16V3-JAP#02-White Dusk WH,CWJB-16V3-JAP#02-White Dusk WH,CWJB-16V3-JAP#02-White Dusk WH,CWJB-16V2-JBP#01-Rolling Sand,CWJB-16V2-JBP#01-Rolling Sand,CWKB-16V1-KPD NVY NAVY Large W,CWKB-16V1-KPD BLK BLACK Small,CWKB-16V1-KPD OWHT OFF WHITE M,CWKB-16V1-KPD BEIGE BEIGE Medi,CWKB-16V1-KPD OWHT OFF WHITE L,CWKB-16V1-KPD NVY NAVY Small W,CWKB-16V1-KPD WHITE WHITE Medi,CWKB-16V1-KPD BLK BLACK Medium,CWKB-16V1-KPD WHITE WHITE Larg,CWKB-16V1-KPD BEIGE BEIGE Larg,CWKB-16V1-KPD OWHT OFF WHITE S,CWKB-16V1-KPD NVY NAVY Medium,CWKB-16V1-KPD WHITE WHITE Smal,CWKB-16V1-KPD BEIGE BEIGE Smal,CWSB-16V2-SBP#03-Ocean Stripe,CWSB-16V2-SBP#03-Ocean Stripe,CWSB-16V2-SBP#03-Ocean Stripe,CWSB-16V3-SPD#31(B)-Scute B BL,CWSB-16V3-SPD#31(B)-Scute B BL,CWSB-16V3-SPD#31(B)-Scute B BL,CWSB-16V3-SPD#31(B)-Scute B BL,CWSB-16V3-SPD#31(B)-Scute B BL,CWSB-FW3-PSB#01-Pallid WHITE L,CWSB-FW3-PSB#01-Pallid WHITE M,CWSB-FW3-PSB#01-Pallid WHITE S,CWSB-16V2-SBP#02-Mermaid BEIGE,CWSB-16V2-SBP#02-Mermaid BEIGE,CMCB-16V2-MWP#01-Men's Trouser,CMCB-16V2-MWP#01-Men's Trouser,CMCB-16V2-MWP#01-Men's Trouser,CCD-DESIGN#01 MULTI COLOR Stan,CCD-DESIGN#04 MULTI COLOR Stan,CCD-DESIGN#05 MULTI COLOR Stan,CCD-DESIGN#06 MULTI COLOR Stan,CCD-DESIGN#07 MULTI COLOR Stan,CCD-DESIGN#09 MULTI COLOR Stan,CWCTB-16V3-CPD#16-Foulard BEIG,CWCTB-16V3-CPD#16-Foulard BEIG,CWCTB-16V3-CPD#16-Foulard BEIG,CWCTB-16V3-CPD#16-Foulard BEIG,CWCTB-16V3-CPD#16-Foulard BEIG,CWS-FW16-DESIGN#01 MULTI COLOR,CWS-FW16-DESIGN#03 MULTI COLOR,CWS-FW16-DESIGN#07 MULTI COLOR,CWS-FW16-DESIGN#08 MULTI COLOR,CWS-FW16-DESIGN#09 MULTI COLOR,CWS-FW16-DESIGN#02 MULTI COLOR,CWS-FW16-DESIGN#04 MULTI COLOR,CWS-FW16-DESIGN#05 MULTI COLOR,CWS-FW16-DESIGN#06 MULTI COLOR,CWS-FW16-DESIGN#10 MULTI COLOR,CWSS-16V2-SSPD#04-Mellow Jungl,CWSS-16V2-SSPD#05-Autumn Berry,CWSS-16V2-SSPD#06-Tribal Orchi,CWCT-16V3-CPD#14-Black Square,CWCT-16V3-PPD#13-Sunset Boulev,CWCT-16V3-PPD#13-Sunset Boulev,CWCT-16V3-PPD#13-Sunset Boulev,CWCT-16V3-PPD#13-Sunset Boulev,CWCT-16V3-PPD#13-Sunset Boulev,CWCT-16V3-CPD#23-Calico BLACK/,CWCT-16V3-CPD#23-Calico BLACK/,CWCT-16V3-CPD#23-Calico BLACK/,CWCT-16V3-CPD#23-Calico BLACK/,CWCT-16V3-CPD#01-Folklore GREY,CWCT-16V3-CPD#01-Folklore GREY,CWCT-16V3-CPD#01-Folklore GREY,CWCT-16V3-CPD#01-Folklore GREY,CWCT-16V3-CPD#08-Medallion BEI,CWCT-16V3-CPD#08-Medallion BEI,CWCT-16V3-CPD#08-Medallion BEI,CWCT-16V3-CPD#08-Medallion BEI,CWCT-16V3-CPD#08-Medallion BEI,CWCT-16V3-CPD#05-Violet Starli,CWCT-16V3-CPD#05-Violet Starli,CWCT-16V3-PPD#17-Rosetta MAROO,CWCT-16V3-PPD#17-Rosetta MAROO,CWCT-16V3-PPD#17-Rosetta MAROO,CWCT-16V3-PPD#17-Rosetta MAROO,CWCT-16V3-PPD#17-Rosetta MAROO,CWCT-16V3-PPD#10-Begonia PINK,CWCT-16V3-PPD#10-Begonia PINK,CWCT-16V3-PPD#10-Begonia PINK,CWCT-16V3-PPD#10-Begonia PINK,CWCT-16V3-PPD#10-Begonia PINK,CWCT-16V3-PDE#09-Duranta BEIGE,CWCT-16V3-PDE#09-Duranta BEIGE,CWCT-16V3-PDE#09-Duranta BEIGE,CWCT-16V3-PDE#09-Duranta BEIGE,CWCT-16V3-PDE#09-Duranta BEIGE,CWCT-16V3-CPD#07-Festive GREEN,CWCT-16V3-CPD#07-Festive GREEN,CWCT-16V3-CPD#07-Festive GREEN,CWCT-16V3-CPD#07-Festive GREEN,CWCT-16V3-CPD#07-Festive GREEN,CWCT-16V3-CPD#04-Shibori BLUE,CWCT-16V3-CPD#04-Shibori BLUE,CWCT-16V3-CPD#04-Shibori BLUE,CWCT-16V3-CPD#04-Shibori BLUE,CWCT-16V3-CPD#09-Rhombus PURPL,CWCT-16V3-CPD#09-Rhombus PURPL,CWCT-16V3-CPD#09-Rhombus PURPL,CWCT-16V3-PDE#02-Afina BLUE XS,CWCT-16V3-PDE#02-Afina BLUE S,CWCT-16V3-PDE#02-Afina BLUE L,CWCT-FW3-CPD#17-Turaco GREEN X,CWCT-FW3-CPD#17-Turaco GREEN X,CWCT-FW3-CPD#17-Turaco GREEN S,CWCT-FW3-CPD#17-Turaco GREEN M,CWCT-FW3-CPD#17-Turaco GREEN L,CWCT-FW3-CPD#18-Bucolic SKIN X,CWCT-FW3-CPD#18-Bucolic SKIN X,CWCT-FW3-CPD#18-Bucolic SKIN S,CWCT-FW3-CPD#18-Bucolic SKIN M,CWCT-FW3-CPD#18-Bucolic SKIN L,CWCT-FW3-PDE#05-Sterling GREY,CWCT-FW3-PDE#05-Sterling GREY,CWCT-FW3-PDE#05-Sterling GREY,CWCT-FW3-PDE#05-Sterling GREY,CWCT-FW3-PDE#05-Sterling GREY,CWCT-16V4-CPD#01-Armeria BLUE,CWCT-16V4-CPD#01-Armeria BLUE,CWCT-16V4-CPD#01-Armeria BLUE,CWCT-16V4-CPD#01-Armeria BLUE,CWCT-16V4-CPD#01-Armeria BLUE,CWCT-16V4-CPD#25-Pop Clash MUL,CWCT-16V4-CPD#25-Pop Clash MUL,CWCT-16V4-CPD#25-Pop Clash MUL,CWCT-16V3-CPD#20-Qashqai BLACK,CWCT-16V3-CPD#20-Qashqai BLACK,CWCT-16V3-CPD#20-Qashqai BLACK,CWCT-16V3-CPD#19-Fall on BLACK,CWCT-16V3-CPD#19-Fall on BLACK,CWCT-16V3-PDE#07-Ogee Crush BL,CWCT-16V3-PDE#07-Ogee Crush BL,CWCT-16V3-PDE#07-Ogee Crush BL,CWCT-16V3-PDE#07-Ogee Crush BL,CWWT-16V4-PPD#20-Dianthus PINK,CWWT-16V4-PPD#20-Dianthus PINK,CWWT-16V4-PPD#20-Dianthus PINK,CWWT-16V4-PPD#20-Dianthus PINK,CWWT-16V4-PPD#20-Dianthus PINK,CWCT-FW3-BK#03-Rustic LIGHT BL,CWCT-FW3-BK#03-Rustic LIGHT BL,CWCT-FW3-BK#03-Rustic LIGHT BL,CWCT-FW3-BK#03-Rustic LIGHT BL,CWCT-FW3-BK#03-Rustic LIGHT BL,CMCT-16V2-MW-PD#04-Impressioni,CMCT-16V2-MW-PD#02-Whirlpool B,CMCT-16V2-MW-PD#02-Whirlpool B,CMCT-16V2-MW-PD#02-Whirlpool B,CMCT-16V2-MW-PD#03-Tribal Ikat,CMCT-16V2-MW-PD#03-Tribal Ikat,CMCT-16V2-MW-PD#03-Tribal Ikat,CMCT-16V2-MW-PD#04-Impressioni,CMCT-16V2-MW-PD#04-Impressioni,CMCT-16V2-MW-YPD#03-Storm Blue,CMCT-16V2-MW-YPD#03-Storm Blue,CMCT-16V2-MW-YPD#03-Storm Blue,CMCT-16V2-MW-YPD#04-Castella G,CMCT-16V2-MW-YPD#04-Castella G,CMCT-16V2-MW-YPD#04-Castella G,CMCT-16V3-MW-YPD#02-Pacific BL,CMCT-16V3-MW-YPD#02-Pacific BL,CMCT-16V3-MW-YPD#02-Pacific BL,CWDT-16V3-BK#01-Lazuli BLUE XL,CWDT-16V3-BK#01-Lazuli BLUE XS,CWDT-16V3-BK#01-Lazuli BLUE S,CWDT-16V3-BK#01-Lazuli BLUE M,CWDT-16V3-BK#01-Lazuli BLUE L,CMJT-16V3-MW-JPD#05-Olivine GR,CMJT-16V3-MW-JPD#05-Olivine GR,CMJT-16V3-MW-JPD#05-Olivine GR,CMJT-16V3-MW-JPD#04-Sand Pebbl,CMJT-16V3-MW-JPD#04-Sand Pebbl,CMJT-16V3-MW-JPD#04-Sand Pebbl,CWJT-16V3-JPD#06-Winter Gold M,CWJT-16V3-JPD#01-Misty Musk LI,CWJT-16V4-JPD#08-Mirth Meld PU,CWJT-16V4-JPD#08-Mirth Meld PU,CWJT-16V4-JPD#08-Mirth Meld PU,CWJT-16V4-JPD#08-Mirth Meld PU,CWJT-16V4-JPD#08-Mirth Meld PU,CWJT-16V4-JPD#11-Regalia PINK,CWJT-16V4-JPD#11-Regalia PINK,CWJT-16V4-JPD#11-Regalia PINK,CWJT-16V4-JPD#11-Regalia PINK,CWJT-16V4-JPD#11-Regalia PINK,CMJT-16V2-JPD# 1-Walnut PEACH,CMJT-16V2-JPD# 1-Walnut PEACH,CMJT-16V2-JPD# 1-Walnut PEACH,CMJT-16V2-MW-JPD#02-Monterey G,CMJT-16V2-MW-JPD#02-Monterey G,CMJT-16V2-MW-JPD#02-Monterey G,CWNT-16V3-EAJD#01-Ottoman Glor,CWNT-16V3-EAJD#01-Ottoman Glor,CWNT-16V3-EAJD#02-Escada PINK,CMJT-16V3-MW-JPD#03-Comet Blue,CMJT-16V3-MW-JPD#03-Comet Blue,CMJT-16V3-MW-JPD#03-Comet Blue,CMJT-16V3-MW-JPD#06-Graphite B,CMJT-16V3-MW-JPD#06-Graphite B,CMJT-16V3-MW-JPD#06-Graphite B,CWKT-16V4-WPD#27-Calla BLUE/RE,CWKT-16V4-WPD#27-Calla BLUE/RE,CWKT-16V4-WPD#27-Calla BLUE/RE,CWKT-16V4-WPD#27-Calla BLUE/RE,CWKT-16V4-WPD#27-Calla BLUE/RE,CWKT-16V4-WPD#28-Rosa Blend RE,CWKT-16V4-WPD#28-Rosa Blend RE,CWKT-16V4-WPD#28-Rosa Blend RE,CWKT-16V4-WPD#28-Rosa Blend RE,CWKT-16V4-WPD#28-Rosa Blend RE,CWKT-16V4-WPD#29-Urban Trip BL,CWKT-16V4-WPD#29-Urban Trip BL,CWKT-16V4-WPD#29-Urban Trip BL,CWKT-16V4-WPD#29-Urban Trip BL,CWKT-16V4-WPD#29-Urban Trip BL,CWKT-16V4-WPD#31-Pinacoid RED/,CWKT-16V4-WPD#31-Pinacoid RED/,CWKT-16V4-WPD#31-Pinacoid RED/,CWKT-16V4-WPD#38-Pedion BLACK/,CWKT-16V4-WPD#38-Pedion BLACK/,CWKT-16V4-WPD#38-Pedion BLACK/,CWKT-16V4-WPD#38-Pedion BLACK/,CWKT-16V4-WPD#38-Pedion BLACK/,CWKT-16V4-WPD#39-Dusk Prism OR,CWKT-16V4-WPD#39-Dusk Prism OR,CWKT-16V4-WPD#39-Dusk Prism OR,CWKT-16V4-WPD#39-Dusk Prism OR,CWKT-16V4-WPD#39-Dusk Prism OR,CWKT-16V4-WPD#40-Acacia GREEN/,CWKT-16V4-WPD#40-Acacia GREEN/,CWKT-16V4-WPD#40-Acacia GREEN/,CWKT-16V4-WPD#40-Acacia GREEN/,CWKT-16V4-WPD#40-Acacia GREEN/,CWKT-16V4-WPD#41-Salvia RED/YE,CWKT-16V4-WPD#41-Salvia RED/YE,CWKT-16V4-WPD#41-Salvia RED/YE,CWKT-16V4-WPD#41-Salvia RED/YE,CWKT-16V4-WPD#41-Salvia RED/YE,CWKT-16V4-WPD#48-Daintree RED/,CWKT-16V4-WPD#48-Daintree RED/,CWKT-16V4-WPD#48-Daintree RED/,CWKT-16V4-WPD#50-Maestoso NAVY,CWKT-16V4-WPD#50-Maestoso NAVY,CWKT-16V4-WPD#50-Maestoso NAVY,CWKT-16V4-WPD#36-Magenta Rippl,CWKT-16V4-WPD#36-Magenta Rippl,CWKT-16V4-WPD#36-Magenta Rippl,CWKT-16V4-WPD#36-Magenta Rippl,CWKT-16V4-WPD#36-Magenta Rippl,CWKT-16V4-WPD#34-Columbine BLU,CWKT-16V4-WPD#34-Columbine BLU,CWKT-16V4-WPD#34-Columbine BLU,CWKT-16V4-WPD#34-Columbine BLU,CWKT-16V4-WPD#34-Columbine BLU,CWKT-16V4-WPD#43-Indigo Cluste,CWKT-16V4-WPD#43-Indigo Cluste,CWKT-16V4-WPD#43-Indigo Cluste,CWKT-16V4-WPD#43-Indigo Cluste,CWKT-16V4-WPD#43-Indigo Cluste,CWKT-16V4-WPD#32-Diplacus RED/,CWKT-16V4-WPD#32-Diplacus RED/,CWKT-16V4-WPD#32-Diplacus RED/,CWKT-16V4-WPD#32-Diplacus RED/,CWKT-16V4-WPD#32-Diplacus RED/,CWLT-16V- PD# 24 DARK BROWN XL,CWLT-16V- PD# 44 OFFWHITE/PURP,CWLT-16V- PD# 33 GREEN/OFFWHIT,CWLT-16V- PD# 24 DARK BROWN La,CWLT-16V- PD# 44 OFFWHITE/PURP,CWLT-16V- PD# 24 DARK BROWN Me,CWLT-16V- PD# 24 DARK BROWN X-,CWLT-16V- PD# 21 BLACK / WHITE,CWLT-16V2-PD# 17-Brocade SKY B,CWLT-16V2-PDE#07-Sunglow YELLO,CWLT-16V2-PDE#07-Sunglow YELLO,CWLT-16V2-PD#36-Blue Tornado G,CMCT-16V2-YPD# 1-WhiteMushroom,CMCT-16V2-YPD# 1-WhiteMushroom,CMCT-16V2-YPD# 1-WhiteMushroom,CMCT-16V2-YPD# 2-Cardamom OFF,CMCT-16V2-YPD# 2-Cardamom OFF,CMCT-16V2-YPD# 2-Cardamom OFF,CWLT-16V2-PD#23-VINTAGE GARDEN,CWLT-16V2-PD#23-VINTAGE GARDEN,CWLT-16V2-PD#23-VINTAGE GARDEN,CWLT-16V2-PDE#10-DANCING LILIE,CWLT-16V2-PDE#10-DANCING LILIE,CWLT-16V2-PDE#10-DANCING LILIE,CWLT-16V2-PDE#10-DANCING LILIE,CWLT-16V2-PD# 52-Foliage GREEN,CWLT-16V2-PDE#08-Hatice SKIN S,CWLT-16V2-PD# 42-SummerEscape,CWLT-16V2-PD#28-Camouflage GRE,CWLT-16V2-PD# 27-OrchidBlush G,CWLT-16V- PD# 44 OFFWHITE/PURP,CWLT-16V- PD# 35 BLACK / WHITE,CWLT-16V- PD# 30 BLUE/GREY Lar,CWLT-16V- PD# 32 GREY/ORANGE L,CWLT-16V- PD# 25 DARK GREY X-S,CWLT-16V1- PD# 07 LIGHT PURPLE,CWLT-16V- PD# 31 BLACK / WHITE,CWLT-16V- PD# 13 BLACK/BROWN X,CWLT-16V- PD# 12 RED/WHITE Lar,CWLT-16V1-PD# 03 BLUE/DARK PIN,CWLT-16V2-PD# 20-AztecRomance,CWLT-16V2-PD# 27-OrchidBlush G,CWLT-16V2-PD#59-Blue Bird AQUA,CWLT-16V2-PD#36-Blue Tornado G,CWLT-16V2-PD# 20-AztecRomance,CWLT-16V2-PD#28-Camouflage GRE,CWLT-16V2-PDE#04-Rouge RED S W,CWLT-16V2-PD#60-Exotic Bird GR,CWLT-16V2-PD#28-Camouflage GRE,CWLT-16V2-PD# 17-Brocade SKY B,CWLT-16V2-PDE#04-Rouge RED L W,CWLT-16V2-PD# 44-Monsoon BLACK,CWLT-16V1-PD# 01 BLACK/GREEN/W,CWLT-16V2-PD#15-Rainfall BLACK,CWLT-16V2-PD#60-Exotic Bird GR,CWLT-16V- PD# 28 BEIGE/GREEN L,CWLT-16V2-PD# 52-Foliage GREEN,CWLT-16V2-PDE#08-Hatice SKIN M,CWLT-16V2-PD# 20-AztecRomance,CWLT-16V2-PD# 42-SummerEscape,CWLT-16V2-PD# 17-Brocade SKY B,CWLT-16V2-PD#28-Camouflage GRE,CWLT-16V2-PDE#04-Rouge RED M W,CWLT-16V2-PD#59-Blue Bird AQUA,CWLT-16V- PD# 28 BEIGE/GREEN M,CWLT-16V2-PDE#08-Hatice SKIN L,CWLT-16V2-PD# 27-OrchidBlush G,CWLT-16V2-PD# 42-SummerEscape,CWLT-16V2-PD# 20-AztecRomance,CWLT-16V2-PD#11-Rain Drop BLUE,CWLT-16V2-PD#11-Rain Drop BLUE,CWLT-16V2-PD#11-Rain Drop BLUE,CWLT-16V3-PD#20-Love Script BE,CWLT-16V3-PD#20-Love Script BE,CWLT-16V3-PD#20-Love Script BE,CWLT-16V3-PD#20-Love Script BE,CWLT-16V3-PD#20-Love Script BE,CWLT-16V2-PD#21-Tribal Dance B,CWLT-16V2-PD#21-Tribal Dance B,CWLT-16V2-PD#21-Tribal Dance B,CWST-16V3-SPD#24-Creek Meadows,CWST-16V3-SPD#24-Creek Meadows,CWST-16V3-SPD#24-Creek Meadows,CWST-16V2-SPD#27-Pearl Lily GR,CWST-16V2-SPD#27-Pearl Lily GR,CWST-16V2-SPD#27-Pearl Lily GR,CWST-16V2-SPD#27-Pearl Lily GR,CWST-16V3-SPD#02-Living Jungle,CWST-16V3-SPD#02-Living Jungle,CWST-16V3-SPD#02-Living Jungle,CWST-16V3-SPD#05-Darling Buds,CWST-16V3-SPD#05-Darling Buds,CWST-16V2-SPD#26-Maypole GREEN,CWST-16V2-SPD#26-Maypole GREEN,CWST-16V2-SPD#26-Maypole GREEN,CWST-16V2-SPD#05-Palampur BEIG,CWST-16V2-SPD#05-Palampur BEIG,CWST-16V2-SPD#05-Palampur BEIG,CWST-16V2-SPD#30-Wild Flower B,CWST-16V3-SPD#16-EnchantedGard,CWST-16V2-SPD#04-Inkblot BLUE/,CWST-16V2-SPD#04-Inkblot BLUE/,CWST-16V2-SPD#04-Inkblot BLUE/,CWST-16V3-SPD#08-Sun Down GREE,CWST-16V3-SPD#08-Sun Down GREE,CWST-16V3-SPD#08-Sun Down GREE,CWST-16V3-SPD#08-Sun Down GREE,CWST-16V3-SPD#08-Sun Down GREE,CWST-16V3-SPD#31-Scute A BLUE,CWST-16V3-SPD#31-Scute A BLUE,CWST-16V3-SPD#31-Scute A BLUE,CWST-16V3-SPD#31-Scute A BLUE,CWST-16V3-SPD#31-Scute A BLUE,CWST-16V3-SPD#26-Jade Vine GRE,CWST-16V3-SPD#26-Jade Vine GRE,CWST-16V3-SPD#26-Jade Vine GRE,CWST-16V3-SPD#26-Jade Vine GRE,CWST-16V3-SPD#26-Jade Vine GRE,CWST-16V3-SPD#15-Stray Morning,CWST-16V3-SPD#15-Stray Morning,CWST-16V3-SPD#15-Stray Morning,CWST-16V3-SPD#33-Senetti PURPL,CWST-16V3-SPD#33-Senetti PURPL,CWST-16V3-SPD#33-Senetti PURPL,CWST-16V3-SPD#33-Senetti PURPL,CWST-16V3-SPD#33-Senetti PURPL,CWST-16V3-SPD#10-Apricot Tan O,CWST-16V3-SPD#10-Apricot Tan O,CWST-16V3-SPD#10-Apricot Tan O,CWST-16V3-SPD#10-Apricot Tan O,CWST-16V3-SPD#10-Apricot Tan O,CWST-16V3-SPD#21-Lavender Mist,CWST-16V3-SPD#21-Lavender Mist,CWST-16V3-SPD#21-Lavender Mist,CWST-16V3-SPD#21-Lavender Mist,CWST-16V3-SPD#21-Lavender Mist,CWST-FW2-SPD#21-Wisteria PURPL,CWST-FW2-SPD#21-Wisteria PURPL,CWST-FW2-SPD#21-Wisteria PURPL,CWST-FW2-SPD#21-Wisteria PURPL,CWST-FW2-SPD#21-Wisteria PURPL,CWST-FW2-SPD#16-Fern Shade GRE,CWST-FW2-SPD#16-Fern Shade GRE,CWST-FW2-SPD#16-Fern Shade GRE,CWST-FW2-SPD#16-Fern Shade GRE,CWST-FW2-SPD#16-Fern Shade GRE,CWST-FW3-SPD#30-Zinnia MAROON,CWST-FW3-SPD#30-Zinnia MAROON,CWST-FW3-SPD#30-Zinnia MAROON,CWST-FW3-SPD#30-Zinnia MAROON,CWST-FW3-SPD#30-Zinnia MAROON,CWST-FW3-SPD#28-Urban Thicket,CWST-FW3-SPD#28-Urban Thicket,CWST-FW3-SPD#28-Urban Thicket,CWST-FW3-SPD#28-Urban Thicket,CWST-FW3-SPD#28-Urban Thicket,CWST-16V4-SPD#01-Suburb Stripe,CWST-16V4-SPD#01-Suburb Stripe,CWST-16V4-SPD#01-Suburb Stripe,CWST-16V4-SPD#01-Suburb Stripe,CWST-16V4-SPD#01-Suburb Stripe,CWST-16V4-SPD#03-Ajacis BLUE X,CWST-16V4-SPD#03-Ajacis BLUE X,CWST-16V4-SPD#03-Ajacis BLUE S,CWST-16V4-SPD#03-Ajacis BLUE M,CWST-16V4-SPD#03-Ajacis BLUE L,CWST-16V4-SPD#13-Sinensis TEAL,CWST-16V4-SPD#13-Sinensis TEAL,CWST-16V4-SPD#13-Sinensis TEAL,CWST-16V4-SPD#13-Sinensis TEAL,CWST-16V4-SPD#13-Sinensis TEAL,CWST-16V2-SPD#17-Chinese Whisp,CWST-16V2-SPD#17-Chinese Whisp,CWST-16V2-SPD#17-Chinese Whisp,CWST-16V2-SPD#03-Majistic Jung,CWST-16V2-SPD#03-Majistic Jung,CWST-16V2-SPD#03-Majistic Jung,CWST-16V2-SPD#22-Pink Delicacy,CWST-16V2-SPD#22-Pink Delicacy,CWST-16V2-SPD#22-Pink Delicacy,CWST-16V2-SPD#60-Anteia's Isla,CWST-16V2-SPD#60-Anteia's Isla,CWST-16V2-SPD#60-Anteia's Isla,CWST-16V2-SPD#60-Anteia's Isla,CWST-16V2-SPD#02-Purple Pebble,CWST-16V2-SPD#02-Purple Pebble,CWST-16V2-SPD#02-Purple Pebble,CWST-16V2-SPD#02-Purple Pebble,CWST-16V2-SPD#02-Purple Pebble,CWST-16V3-SPD#14-Wine Yard GRE,CWST-16V3-SPD#14-Wine Yard GRE,CWST-16V3-SPD#14-Wine Yard GRE,CWST-16V3-SPD#12-BlackMystery,CWST-16V3-SPD#12-BlackMystery,CWST-16V3-SPD#12-BlackMystery,CWWT-16V4-PPD#25-Sanguine TEAL,CWWT-16V4-PPD#25-Sanguine TEAL,CWWT-16V4-PPD#25-Sanguine TEAL,CWWT-16V4-PPD#25-Sanguine TEAL,CWWT-16V4-PPD#25-Sanguine TEAL,CWWT-16V4-PPD#21-Ecru OFF WHIT,CWWT-16V4-PPD#21-Ecru OFF WHIT,CWWT-16V4-PPD#21-Ecru OFF WHIT,CWWT-16V4-PPD#21-Ecru OFF WHIT,CWWT-16V4-PPD#21-Ecru OFF WHIT,CWWT-16V4-PPD#23-Reseda GREEN,CWWT-16V4-PPD#23-Reseda GREEN,CWWT-16V4-PPD#23-Reseda GREEN,CWWT-16V4-PPD#23-Reseda GREEN,CWWT-16V4-PPD#23-Reseda GREEN,CWWT-16V4-PPD#22-Persica PINK,CWWT-16V4-PPD#22-Persica PINK,CWWT-16V4-PPD#22-Persica PINK,CWWT-16V4-PPD#22-Persica PINK,CWWT-16V4-PPD#22-Persica PINK,CWWT-16V4-PPD#19-Reticent GREY,CWWT-16V4-PPD#19-Reticent GREY,CWWT-16V4-PPD#19-Reticent GREY,CWWT-16V4-PPD#19-Reticent GREY,CWWT-16V4-PPD#19-Reticent GREY,CWCT-16V3-CPD#21-Persian Dynas,CWCT-16V3-CPD#21-Persian Dynas,CWCT-16V3-CPD#21-Persian Dynas,CWCT-16V3-CPD#21-Persian Dynas,CWCT-16V3-CPD#06-Hibiscus GREY,CWCT-16V3-CPD#06-Hibiscus GREY,CWCT-16V3-CPD#06-Hibiscus GREY,CWCT-16V3-CPD#06-Hibiscus GREY,CWCT-16V3-CPD#06-Hibiscus GREY,CWCT-16V3-CPD#02-Larkspur BLUE,CWCT-16V3-CPD#02-Larkspur BLUE,CWCT-16V3-CPD#02-Larkspur BLUE,CWCT-16V3-CPD#02-Larkspur BLUE,CWCT-16V3-CPD#15-TurkishStripp,CWCT-16V3-CPD#15-TurkishStripp,CWCT-16V3-CPD#15-TurkishStripp,CWCT-16V3-CPD#15-TurkishStripp,CWCT-16V3-CPD#15-TurkishStripp,CWCT-16V3-CPD#13-Madras Ikat B,CWCT-16V3-CPD#13-Madras Ikat B,CWCT-16V3-CPD#13-Madras Ikat B,CWCT-16V3-CPD#13-Madras Ikat B,CWCT-16V3-CPD#22-Blotch BLACK,CWCT-16V3-CPD#22-Blotch BLACK,CWCT-16V3-CPD#22-Blotch BLACK,CWCT-16V3-CPD#22-Blotch BLACK,CWJT-16V3-JPD#05-Jujube GREEN,CWJT-16V3-JPD#05-Jujube GREEN,CWJT-16V3-JPD#05-Jujube GREEN,CWJT-16V3-JPD#05-Jujube GREEN,CWJT-16V3-JPD#05-Jujube GREEN,CWJT-16V3-JPD#03-Dahlia ORANGE,CWJT-16V3-JPD#03-Dahlia ORANGE,CWJT-16V3-JPD#03-Dahlia ORANGE,CWJT-16V3-JPD#03-Dahlia ORANGE,CWLT-16V2-PD#53-Go Green GREEN,CWLT-16V2-PD#53-Go Green GREEN,CWLT-16V2-PD#53-Go Green GREEN,CWLT-16V2-PD#53-Go Green GREEN,CWLT-16V2-PD#53-Go Green GREEN,CWLT-16V2-PDE#02-Mint Rose MIN,CWLT-16V2-PDE#02-Mint Rose MIN,CWLT-16V2-PDE#02-Mint Rose MIN,CWLT-16V2-PDE#02-Mint Rose MIN,CWLT-16V2-PD#16-Mirage PINK/BL,CWLT-16V2-PD#16-Mirage PINK/BL,CWLT-16V2-PD#16-Mirage PINK/BL,CWLT-16V2-PD#16-Mirage PINK/BL,CWLT-16V2-PD#66-Illusionism WH,CWLT-16V2-PD#66-Illusionism WH,CWLT-16V2-PD#66-Illusionism WH,CWLT-16V2-PD#66-Illusionism WH,CWLT-16V2-PD#56-Midnight Sky B,CWLT-16V2-PD#56-Midnight Sky B,CWLT-16V2-PD#56-Midnight Sky B,CWLT-16V2-PD#56-Midnight Sky B,CWSB-16V2-ESBP#01-Addiction BE,CWSB-16V2-ESBP#01-Addiction BE,CWSB-16V2-ESBP#01-Addiction BE,CWCT-16V3-PPD#15-French Rose P,CWCT-16V3-PPD#15-French Rose P,CWCT-16V3-PPD#15-French Rose P,CMCT-16V3-MW-YPD#01-Gloss Grey,CMCT-16V3-MW-YPD#01-Gloss Grey,CMCT-16V3-MW-YPD#01-Gloss Grey,CMCT-16V3-MW-YPD#08-Stormae BR,CMCT-16V3-MW-YPD#08-Stormae BR,CMCT-16V3-MW-YPD#08-Stormae BR,CMJT-16V3-MW-JPD#04-Sand Pebbl,CMJT-16V3-MW-JPD#04-Sand Pebbl,CMJT-16V3-MW-JPD#04-Sand Pebbl,CMJT-16V3-MW-JPD#03-Comet Blue,CMJT-16V3-MW-JPD#03-Comet Blue,CMJT-16V3-MW-JPD#03-Comet Blue,CWJT-16V3-JPD#01-Misty Musk LI,CWJT-16V3-JPD#01-Misty Musk LI,CWJT-16V3-JPD#01-Misty Musk LI,CWLT-16V2-PD#38-Sun Shower OLI,CWLT-16V2-PD#38-Sun Shower OLI,CWLT-16V2-PD#38-Sun Shower OLI,CWLT-16V2-PD#13-Mosaic RED/GRE,CWLT-16V2-PD#13-Mosaic RED/GRE,CWLT-16V2-PD#13-Mosaic RED/GRE,CWST-16V3-SPD#18-Marine Sakura,CWST-16V3-SPD#18-Marine Sakura,CWST-16V3-SPD#18-Marine Sakura,CWST-16V3-SPD#17-Morocco Flair,CWST-16V3-SPD#17-Morocco Flair,CWST-16V3-SPD#17-Morocco Flair,CWST-16V3-SPD#06-Night Fall BL,CWST-16V3-SPD#06-Night Fall BL,CWST-16V3-SPD#06-Night Fall BL,CWST-16V3-SPD#07-Dancing Dahli,CWST-16V3-SPD#07-Dancing Dahli,CWST-16V3-SPD#07-Dancing Dahli,CWST-16V3-SPD#35-Tiffany Gold,CWST-16V3-SPD#35-Tiffany Gold,CWST-16V3-SPD#35-Tiffany Gold,CMCT-16V3-MW-YPD#03-Cinereal G,CMCT-16V3-MW-YPD#03-Cinereal G,CMCT-16V3-MW-YPD#03-Cinereal G,CWCB-16V1-AP 05 WHITE Small Wo,CWCB-16V1-AP 05 WHITE Medium W,CWCB-16V1-AP 05 WHITE Large Wo,CWCB-16V1-AP 02 BEIGE Small Wo,CWCB-16V1-AP 02 BEIGE Medium W,CWSS-16V2-SSPD#01-Arabesque BR,CWSS-16V3-SSPD#02-Kaleidoscope,CWSS-16V3-SSPD#03-Coral Reef M,CWSS-16V3-SSPD#04-Bulbinella R,CWSS-16V3-SSPD#05-Blue Iris BL,CWSS-16V3-SSPD#06-Boraginace B,CWLT-16V3-PDE#08-Aureate BEIGE,CWLT-16V3-PDE#08-Aureate BEIGE,CWLT-16V2-PD# 61-Impression PU,CWLT-16V2-PD# 61-Impression PU,CWLT-16V2-PD#22-Gold Boulevard,CWLT-16V2-PD#22-Gold Boulevard,CWST-16V3-SPD#29-Atlantis TEAL,CWST-16V3-SPD#29-Atlantis TEAL,CWST-16V3-SPD#19-Boysen Berry,CWST-16V3-SPD#19-Boysen Berry,CWST-16V2-SPD#11-Dream Tree BE,CWST-16V2-SPD#11-Dream Tree BE,CMCT-16V2-MW-PD#05-Cracker OLI,CMCT-16V2-MW-PD#05-Cracker OLI";
            string foundStrings = "CWSB-16V3-SPD#31(B)-Scute B BL,CWCTB-16V3-CPD#16-Foulard BEIG,CWCT-16V3-CPD#07-Festive GREEN,CWST-16V3-SPD#10-Apricot Tan O,CWSS-16V2-SSPD#05-Autumn Berry,CWCT-16V3-PPD#10-Begonia PINK,CWCT-16V3-PDE#09-Duranta BEIGE,CWCT-16V3-CPD#01-Folklore GREY,CWST-16V3-SPD#26-Jade Vine GRE,CWST-16V3-SPD#21-Lavender Mist,CWST-16V2-SPD#26-Maypole GREEN,CWCT-16V3-CPD#09-Rhombus PURPL,CWJB-16V2-JBP#01-Rolling Sand,CWST-16V3-SPD#31-Scute A BLUE,CWST-16V3-SPD#33-Senetti PURPL,CWCT-16V3-CPD#04-Shibori BLUE,CWST-16V3-SPD#15-Stray Morning,CWLT-16V2-PD# 42-SummerEscape,CWST-16V3-SPD#08-Sun Down GREE,CWLT-16V2-PDE#07-Sunglow YELLO,CWJB-16V3-JAP#02-White Dusk WH,CWSS-16V2-SSPD#04-Mellow Jungl,CWSS-16V2-SSPD#06-Tribal Orchi,CWSS-16V2-SSPD#06-Tribal Orchi,CCD-DESIGN#09 MULTI COLOR Stan,CCD-DESIGN#07 MULTI COLOR Stan,CCD-DESIGN#06 MULTI COLOR Stan,CCD-DESIGN#04 MULTI COLOR Stan,CCD-DESIGN#01 MULTI COLOR Stan,CMCB-16V2-MWP#01-Men's Trouser,CMCT-16V2-YPD# 2-Cardamom OFF,CMCT-16V2-MW-YPD#04-Castella G,CMJT-16V3-MW-JPD#06-Graphite B,CMCT-16V2-MW-PD#04-Impressioni,CMJT-16V2-MW-JPD#02-Monterey G,CMJT-16V3-MW-JPD#05-Olivine GR,CMCT-16V3-MW-YPD#02-Pacific BL,CMCT-16V2-MW-YPD#03-Storm Blue,CMCT-16V2-MW-PD#03-Tribal Ikat,CMJT-16V2-JPD# 1-Walnut PEACH,CMCT-16V2-MW-PD#02-Whirlpool B,CMCT-16V2-YPD# 1-WhiteMushroom,CWCB-16V3-EAP#01-Pearlesque WH,CWJB-16V2-JBP#01-Rolling Sand,CWSB-16V2-SBP#03-Ocean Stripe,CWLT-16V2-PD# 20-AztecRomance,CWLT-16V2-PD#59-Blue Bird AQUA,CWLT-16V2-PD# 17-Brocade SKY B,CWLT-16V2-PD#28-Camouflage GRE,CWLT-16V2-PDE#10-DANCING LILIE,CWLT-16V2-PD#60-Exotic Bird GR,CWLT-16V2-PD# 52-Foliage GREEN,CWLT-16V3-PD#20-Love Script BE,CWLT-16V2-PD# 44-Monsoon BLACK,CWLT-16V2-PD# 27-OrchidBlush G,CWLT-16V2-PD#11-Rain Drop BLUE,CWLT-16V2-PD#15-Rainfall BLACK,CWLT-16V2-PDE#04-Rouge RED L W,CWLT-16V2-PD#21-Tribal Dance B,CWLT-16V2-PD#23-VINTAGE GARDEN,CWS-FW16-DESIGN#10 MULTI COLOR,CWS-FW16-DESIGN#09 MULTI COLOR,CWS-FW16-DESIGN#08 MULTI COLOR,CWS-FW16-DESIGN#07 MULTI COLOR,CWS-FW16-DESIGN#06 MULTI COLOR,CWS-FW16-DESIGN#05 MULTI COLOR,CWS-FW16-DESIGN#04 MULTI COLOR,CWS-FW16-DESIGN#03 MULTI COLOR,CWS-FW16-DESIGN#02 MULTI COLOR,CWS-FW16-DESIGN#01 MULTI COLOR,CWST-16V2-SPD#60-Anteia's Isla,CWST-16V3-SPD#12-BlackMystery,CWCT-16V3-CPD#14-Black Square,CWST-16V2-SPD#17-Chinese Whisp,CWST-16V3-SPD#24-Creek Meadows,CWST-16V3-SPD#05-Darling Buds,CWST-16V3-SPD#16-EnchantedGard,CWNT-16V3-EAJD#02-Escada PINK,CWCT-16V3-CPD#19-Fall on BLACK,CWST-16V3-SPD#02-Living Jungle,CWST-16V2-SPD#03-Majistic Jung,CWCT-16V3-CPD#08-Medallion BEI,CWCT-16V3-PDE#07-Ogee Crush BL,CWNT-16V3-EAJD#01-Ottoman Glor,CWST-16V2-SPD#05-Palampur BEIG,CWST-16V2-SPD#27-Pearl Lily GR,CWST-16V2-SPD#22-Pink Delicacy,CWST-16V2-SPD#02-Purple Pebble,CWCT-16V3-CPD#20-Qashqai BLACK,CWCT-16V3-PPD#17-Rosetta MAROO,CWCT-16V3-PPD#13-Sunset Boulev,CWCT-16V3-CPD#05-Violet Starli,CWST-16V2-SPD#30-Wild Flower B,CWST-16V3-SPD#14-Wine Yard GRE,CWWT-16V4-PPD#23-Reseda GREEN,CWWT-16V4-PPD#21-Ecru OFF WHIT,CWWT-16V4-PPD#25-Sanguine TEAL,CWWT-16V4-PPD#22-Persica PINK,CWWT-16V4-PPD#19-Reticent GREY,CWKT-16V4-WPD#50-Maestoso NAVY,CWKT-16V4-WPD#48-Daintree RED/,CWKT-16V4-WPD#41-Salvia RED/YE,CWKT-16V4-WPD#40-Acacia GREEN/,CWKT-16V4-WPD#39-Dusk Prism OR,CWKT-16V4-WPD#38-Pedion BLACK/,CWKT-16V4-WPD#31-Pinacoid RED/,CWKT-16V4-WPD#29-Urban Trip BL,CWKT-16V4-WPD#28-Rosa Blend RE,CWKT-16V4-WPD#27-Calla BLUE/RE,CWWT-16V4-PPD#20-Dianthus PINK,CWJT-16V4-JPD#08-Mirth Meld PU,CWCT-16V3-CPD#15-TurkishStripp,CWCT-16V3-CPD#02-Larkspur BLUE,CWJT-16V3-JPD#01-Misty Musk LI,CWJT-16V3-JPD#03-Dahlia ORANGE,CWST-16V2-SPD#11-Dream Tree BE,CWST-16V3-SPD#29-Atlantis TEAL,CWST-16V3-SPD#18-Marine Sakura,CMJT-16V3-MW-JPD#03-Comet Blue,CMCT-16V3-MW-YPD#08-Stormae BR,CWSB-16V2-SBP#02-Mermaid BEIGE,CWSS-16V3-SSPD#06-Boraginace B,CWSS-16V3-SSPD#05-Blue Iris BL,CWSS-16V3-SSPD#04-Bulbinella R,CWSS-16V3-SSPD#03-Coral Reef M,CWSS-16V3-SSPD#02-Kaleidoscope,CWKB-16V1-KPD BEIGE BEIGE Smal,CWKB-16V1-KPD BLK BLACK Small,CWKB-16V1-KPD NVY NAVY Small W,CWKB-16V1-KPD OWHT OFF WHITE S,CWKB-16V1-KPD WHITE WHITE Medi,CWSS-16V2-SSPD#01-Arabesque BR,CWKT-16V4-WPD#34-Columbine BLU,CWJT-16V4-JPD#11-Regalia PINK,CMCT-16V3-MW-YPD#03-Cinereal G,CMCT-16V2-MW-PD#05-Cracker OLI,CMCT-16V3-MW-YPD#01-Gloss Grey,CMJT-16V3-MW-JPD#04-Sand Pebbl,CWSB-16V2-ESBP#01-Addiction BE,CWCB-16V1-AP 02 BEIGE Small Wo,CWCB-16V1-AP 05 White Small Wo,CWST-16V3-SPD#19-Boysen Berry,CWST-16V3-SPD#07-Dancing Dahli,CWCT-16V3-CPD#13-Madras Ikat B,CWST-16V3-SPD#17-Morocco Flair,CWST-16V3-SPD#06-Night Fall BL,CWST-16V3-SPD#35-Tiffany Gold,CWLT-16V3-PDE#08-Aureate BEIGE,CWJT-16V3-JPD#05-Jujube GREEN,CWCT-16V3-CPD#22-Blotch BLACK,CWLT-16V2-PD#53-Go Green GREEN,CWCT-16V3-PPD#15-French Rose P,CWLT-16V2-PD#22-Gold Boulevard,CWCT-16V3-CPD#06-Hibiscus GREY,CWLT-16V2-PD#66-Illusionism WH,CWLT-16V2-PD# 61-Impression PU,CWCT-16V3-CPD#21-Persian Dynas,CWLT-16V2-PD#56-Midnight Sky B,CWLT-16V2-PDE#02-Mint Rose MIN,CWLT-16V2-PD#16-Mirage PINK/BL,CWLT-16V2-PD#13-Mosaic RED/GRE,CWLT-16V2-PD#38-Sun Shower OLI,CWST-FW2-SPD#16-Fern Shade GRE,CWST-FW2-SPD#21-Wisteria PURPL,CWST-FW3-SPD#30-Zinnia MAROON,CWCT-FW3-BK#03-Rustic LIGHT BL,CWKT-16V4-WPD#43-Indigo Cluste,CWCT-16V4-CPD#01-Armeria BLUE,CWCT-FW3-CPD#18-Bucolic SKIN S,CWCT-16V4-CPD#25-Pop Clash MUL,CWCT-FW3-PDE#05-Sterling GREY,CWCT-FW3-CPD#17-Turaco GREEN X";

            string notfound = "";
            foreach (var item in fileStrings.Split(','))
            {

                if(!notfound.Contains(item))
                    notfound = notfound + (foundStrings.Split(',').Where(x => x == item).Count() == 0 ? item + "," : "");
            }

            return notfound;
        }
    }
}