using FoxLib;
using FoxModel;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using UJia.DAL;

namespace FoxTool
{
    public partial class MainForm : Form
    {
        Web3 web3;
        FoxCoreService foxCore;
        SiringClockAuctionService foxSiring;
        SaleClockAuctionService foxSale;
        string senderAddress = FoxConfig.CEO.Address;
        HexBigInteger gas = new HexBigInteger(1000000);
        HexBigInteger gasPrice = new HexBigInteger(3000000000);
        void InitWeb3()
        {
            var privateKey = FoxConfig.CEO.PrivateKey;

            var account = new Nethereum.Web3.Accounts.Account(privateKey);

            web3 = new Web3(account, FoxConfig.Server);

            foxCore = new FoxCoreService(web3, FoxConfig.CoreAddress);
            foxSale = new SaleClockAuctionService(web3, FoxConfig.SaleClockAuctionAddress);
            foxSiring = new SiringClockAuctionService(web3, FoxConfig.SiringClockAuctionAddress);

        }
        public delegate void SetTextBoxValue(string value);
        void SetMyTextBoxValue(string value)
        {
            // Control.InvokeRequired 属性： 获取一个值，该值指示调用方在对控件进行方法调用时是否必须调用 Invoke 方法，因为调用方位于创建控件所在的线程以外的线程中。当前线程不是创建控件的线程时为true,当前线程中访问是False
            if (this.textBox1.InvokeRequired)
            {
                SetTextBoxValue objSetTextBoxValue = new SetTextBoxValue(SetMyTextBoxValue);

                // IAsyncResult 接口：表示异步操作的状态。不同的异步操作需要不同的类型来描述，自然可以返回任何对象。
                // Control.BeginInvoke 方法 (Delegate)：在创建控件的基础句柄所在线程上异步执行指定委托。
                IAsyncResult result = this.textBox1.BeginInvoke(objSetTextBoxValue, new object[] { value });
                try
                {
                    objSetTextBoxValue.EndInvoke(result);
                }
                catch
                {
                }
            }
            else
            {
                this.textBox1.Text += value + Environment.NewLine;
                this.textBox1.SelectionStart = this.textBox1.TextLength;
                this.textBox1.ScrollToCaret();

            }
        }
        public MainForm()
        {
            InitializeComponent();
            DateTime dtstart = Convert.ToDateTime(UJia.DAL.DbHelperSQL.GetSingle(" select top 1 addtime from trecive where intype=0  order by id desc"));
            this.label1.Text = "上期结算至:" + dtstart.ToString();
        }
        Thread objThread;
        #region 生成收益
        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            objThread = new Thread(new ThreadStart(createMoney));
            objThread.IsBackground = false;
            objThread.Start();

            //   decimal[] 单个用户收益 = 单个用户有效喂养费用 * 单个用户有效魅力 * (总共手续费手收 * 0.95 + 总共喂养费用) / 单个用户有效魅力 * 单个用户有效魅力.Length;
        }

        void createMoney()
        {
            //DateTime dtstart = DateTime.Parse("2018-03-17 00:00:00");// Convert.ToDateTime(UJia.DAL.DbHelperSQL.GetSingle(" select top 1 addtime from trecive  order by addtime desc"));
            //if (true)//dtstart <= DateTime.Now.AddDays(-1))
                DateTime dtstart = Convert.ToDateTime(UJia.DAL.DbHelperSQL.GetSingle(" select top 1 addtime from trecive where intype=0  order by addtime desc"));
            if ( dtstart <= DateTime.Now.AddDays(-1))
            {

                string starttime = dtstart.ToString("yyyy-MM-dd");


                string endtime = dtstart.AddDays(1).ToString("yyyy-MM-dd");
                BigInteger 总共手续费手收 = 0;
                BigInteger 总共流浪费用 = 0;
                BigInteger 总共喂养费用 = 0;
                BigInteger 本次总共有效喂养费用 = 0;
                BigInteger 总送出 = 0;
                string alsql = "select MIN(tokenid) as tokenid,SUM(cast(eth  as bigint)) as feed from tFeedLog where sta =1   and addtime<='" + endtime + " 00:00:00'  and addtime>'" + starttime + " 00:00:00' group by tokenid";
                DataTable 所有喂养 = UJia.DAL.DbHelperSQL.Query(alsql).Tables[0];
                string als = Convert.ToString(UJia.DAL.DbHelperSQL.GetSingle(" select sum(cast(eth as bigint)) from tbuylog where sta=1 and addtime<='" + endtime + " 00:00:00'  and addtime>'" + starttime + " 00:00:00' "));
                if (als == "")
                {
                    als = "0";
                }
                总共手续费手收 = BigInteger.Parse(als) / 20;
                int 参与人数 = 所有喂养.Rows.Count;
                BigInteger[] 单个用户喂养费用 = new BigInteger[参与人数];
                BigInteger[] 单个用户有效喂养费用 = new BigInteger[参与人数];
                BigInteger[] 单个用户爆击率 = new BigInteger[参与人数];
                BigInteger[] 单个用户有效魅力 = new BigInteger[参与人数];
                BigInteger[] 单个用户剩余费用 = new BigInteger[参与人数];
                BigInteger[] 单个用户参与比 = new BigInteger[参与人数];
                BigInteger[] 单个用户收益 = new BigInteger[参与人数];
                BigInteger 总分母 = 0;
                BigInteger alllucky = 0;
                BigInteger allgrow = 0;

                for (int i = 0; i < 参与人数; i++)
                {
                    FoxModel.tFoxBase fox = new FoxModel.tFoxBase(所有喂养.Rows[i]["tokenid"].ToString());
                    string[] genes = fox.genes.Split(',');
                    BigInteger grow = BigInteger.Parse(genes[3]) / 1000000000000 % 1000;
                    BigInteger locky = BigInteger.Parse(genes[3]) / 1000 % 1000;
                    alllucky += locky;
                    allgrow += grow;
                }
                for (int i = 0; i < 参与人数; i++)
                {
                    if (所有喂养.Rows[i]["tokenid"].ToString() == "444")
                    {

                    }
                    单个用户喂养费用[i] = BigInteger.Parse(所有喂养.Rows[i]["feed"].ToString());
                    总共喂养费用 += 单个用户喂养费用[i];
                    //查出上次剩余
                    string sc = Convert.ToString(UJia.DAL.DbHelperSQL.GetSingle("select  top 1 lefteth from trecive where tokenid=" + 所有喂养.Rows[i]["tokenid"].ToString() + " order by id desc"));
                    if (sc != "")
                    {
                        BigInteger shangci = BigInteger.Parse(sc);
                        单个用户喂养费用[i] = 单个用户喂养费用[i] + shangci;
                    }

                    if (单个用户喂养费用[i] <= 50000000000000000)
                    {
                        单个用户有效喂养费用[i] = 单个用户喂养费用[i];
                        单个用户剩余费用[i] = 0;
                    }
                    else
                    {
                        单个用户有效喂养费用[i] = 50000000000000000;
                        单个用户剩余费用[i] = 单个用户喂养费用[i] - 50000000000000000;
                    }
                    本次总共有效喂养费用 += 单个用户有效喂养费用[i];
                    FoxModel.tFoxBase fox = new FoxModel.tFoxBase(所有喂养.Rows[i]["tokenid"].ToString());
                    string[] genes = fox.genes.Split(',');

                    BigInteger grow = BigInteger.Parse(genes[3]) / 1000000000000 % 1000;
                    BigInteger locky = BigInteger.Parse(genes[3]) / 1000 % 1000;
                    BigInteger avglucky = BigInteger.Parse(((decimal)alllucky / (decimal)参与人数).ToString("0"));
                    BigInteger avggrow = BigInteger.Parse(((decimal)allgrow / (decimal)参与人数).ToString("0"));
                    单个用户爆击率[i] = new Random().Next(100, 120);
                    if (locky > avglucky && grow < avggrow)
                    {


                        if (单个用户爆击率[i] <= 110)
                        {
                            单个用户爆击率[i] += new Random().Next(1, 8);
                        }

                    }
                    if (所有喂养.Rows[i]["tokenid"].ToString() == "444")
                    {
                        单个用户爆击率[i] = new Random().Next(100, 109);

                    }
                    if (所有喂养.Rows[i]["tokenid"].ToString() == "397")
                    {
                        单个用户爆击率[i] = new Random().Next(100, 110);

                    }
                    if (所有喂养.Rows[i]["tokenid"].ToString() == "1175")
                    {
                        单个用户爆击率[i] = new Random().Next(100, 113);

                    }
                    单个用户有效魅力[i] = grow * 单个用户有效喂养费用[i];

                    Thread.Sleep(120);
                    单个用户参与比[i] = 单个用户有效魅力[i] * 单个用户爆击率[i];
                    总分母 += 单个用户参与比[i];
                }

                for (int i = 0; i < 参与人数; i++)
                {


                    //      单个用户收益[i] = (单个用户参与比[i] * (本次总共有效喂养费用 + 总共手续费手收)*95/100 ) / 总分母;
                    单个用户收益[i] = (单个用户参与比[i] * (本次总共有效喂养费用 + 总共手续费手收)) / 总分母;
                    总送出 += 单个用户收益[i];

                    string kz = 单个用户有效喂养费用[i] > 单个用户收益[i] ? "亏" : "";
                    string val = 所有喂养.Rows[i]["tokenid"].ToString() + "共喂了" + FoxLib.Tools.GetEtherFromWei(单个用户喂养费用[i]) + ",有效" + FoxLib.Tools.GetEtherFromWei(单个用户有效喂养费用[i]) + ",剩余下次费用" + 单个用户剩余费用[i] + ",有效魅力为:" + 单个用户有效魅力[i] + ",本次爆机" + 单个用户爆击率[i] + ",收益为" + FoxLib.Tools.GetEtherFromWei(单个用户收益[i]) + " " + kz;

                    FoxModel.trecive rec = new FoxModel.trecive();

                    rec.addtime = DateTime.Parse(endtime);
                    rec.allmoney = 单个用户喂养费用[i].ToString();
                    rec.tokenid = 所有喂养.Rows[i]["tokenid"].ToString();
                    rec.thispersnt = (decimal)单个用户爆击率[i] / 100;
                    rec.thiseth = 单个用户收益[i].ToString();
                    rec.thischarm = (decimal)单个用户有效魅力[i] / 10000000000000000;
                    rec.sta = 0;
                    rec.lefteth = 单个用户剩余费用[i].ToString();
                    rec.Add();
                    SetMyTextBoxValue(val);
                }
                if (参与人数 == 0)
                {
                    FoxModel.trecive rec = new FoxModel.trecive();

                    rec.addtime = DateTime.Parse(endtime);
                    rec.allmoney = "0";
                    rec.tokenid = "0";
                    rec.thispersnt = 1;
                    rec.thiseth = "0";

                    rec.thischarm = 0;
                    rec.sta = 1;
                    rec.lefteth = "0";
                    rec.Add();
                }
                SetMyTextBoxValue("手续费:" + FoxLib.Tools.GetEtherFromWei(总共手续费手收) + " 总送出" + FoxLib.Tools.GetEtherFromWei(总送出) + "总有效:" + FoxLib.Tools.GetEtherFromWei(本次总共有效喂养费用) + " 总共喂" + FoxLib.Tools.GetEtherFromWei(总共喂养费用));
                dtstart = Convert.ToDateTime(UJia.DAL.DbHelperSQL.GetSingle(" select top 1 addtime from trecive  order by id desc"));

                MessageBox.Show("结算OK");
            }
            else
            {
                MessageBox.Show("结算时间未到");
            }
        }


        #endregion
    
        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            objThread = new Thread(new ParameterizedThreadStart(createFox));
            objThread.IsBackground = false;
            objThread.Start();
        }

        string[] sss()
        {
            Random rd = new Random();
            //外观 
            string[] gens = new string[6];

            int nd = rd.Next(1, 40);
            gens[0] = nd.ToString().PadLeft(4, '0') + rd.Next(1, 40).ToString().PadLeft(4, '0') + rd.Next(1, 100).ToString().PadLeft(4, '0') + rd.Next(1, 40).ToString().PadLeft(4, '0');
            gens[1] = nd.ToString().PadLeft(4, '0') + rd.Next(1, 40).ToString().PadLeft(4, '0') + rd.Next(1, 40).ToString().PadLeft(4, '0') + rd.Next(1, 40).ToString().PadLeft(4, '0');

            /// [2] 扩展属性  天赋1~999   精力 1~999   智力001-999  力量001~999  敏捷 1~999
            /// 
            /// [3] 成长基因100~999 冷却基因100~999   战斗基因 0~999  运气1~999    ?1-999
            //  [4]  流浪积分 1-99999999
            /// [5] 出生地 000~999, 性格 1~999  技能 1~999  爱好-999    ??? 1-999   


            int a1, a2, a3, a4, a5, a6;
            getGs(out a1, out a2, out a3, 0);

            getGs(out a4, out a5, out a6, 0);

            gens[2] = a1.ToString() + a2.ToString() + a4.ToString() + a5.ToString() + a6.ToString();
            int b1, b2, b3, b4, b5, b6;
            getGs(out b1, out b2, out b3, 0);
            getGs(out b4, out b5, out b6, 0);

            gens[3] = b1.ToString() + (Decimal.Round(500 / decimal.Parse(b2.ToString()) * 100, 0)).ToString() + b3.ToString() + b5.ToString() + b6.ToString();
            gens[4] = "0";
            rd = new Random();
            gens[5] = rd.Next(0, 500).ToString().PadLeft(3, '0') + rd.Next(0, 500).ToString().PadLeft(3, '0') + rd.Next(0, 500).ToString().PadLeft(3, '0') + rd.Next(0, 500).ToString().PadLeft(3, '0') + rd.Next(0, 500).ToString().PadLeft(3, '0');
            return gens;
        }
        void getGs(out int a1, out int a2, out int a3, int id)
        {

            Random rd = new Random();
            int t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12;
            t1 = 400 - 60;
            t2 = 450 - 60;

            t3 = 450 - 60;
            t4 = 500 - 60;
            t5 = 420 - 60;
            t6 = 480 - 60;

            t7 = 440 - 60;
            t8 = 480 - 60;
            t9 = 320 - 60;
            t10 = 350 - 60;
            t11 = 440 - 60;
            t12 = 480 - 60;
            rd = new Random();



            int ind = rd.Next(1, 4);

            if (ind == 1)
            {
                a1 = rd.Next(t1, t2);
                a2 = rd.Next(t1, t2);
                a3 = rd.Next(t1, t2);
            }

            else if (ind == 2)
            {
                int p = rd.Next(1, 4);
                if (p == 1)
                {
                    a1 = rd.Next(t3, t4);
                    int f = rd.Next(1, 3);
                    if (f == 1)
                    {
                        a2 = rd.Next(t5, t6);
                        a3 = rd.Next(t7, t8);
                    }
                    else
                    {
                        a3 = rd.Next(t5, t6);
                        a2 = rd.Next(t7, t8);
                    }
                }
                else if (p == 2)
                {
                    a2 = rd.Next(t3, t4);
                    int f = rd.Next(1, 3);
                    if (f == 1)
                    {
                        a1 = rd.Next(t5, t6);
                        a3 = rd.Next(t7, t8);
                    }
                    else
                    {
                        a3 = rd.Next(t5, t6);
                        a1 = rd.Next(t7, t8);
                    }
                }
                else
                {
                    a3 = rd.Next(t3, t4);
                    int f = rd.Next(1, 3);
                    if (f == 1)
                    {
                        a1 = rd.Next(t5, t6);
                        a2 = rd.Next(t7, t8);
                    }
                    else
                    {
                        a2 = rd.Next(t5, t6);
                        a1 = rd.Next(t7, t8);
                    }
                }

            }
            else
            {
                int p = rd.Next(1, 4);
                if (p == 1)
                {
                    a1 = rd.Next(t7, t8);
                    int f = rd.Next(1, 3);
                    if (f == 1)
                    {
                        a2 = rd.Next(t9, t10);
                        a3 = rd.Next(t11, t12);
                    }
                    else
                    {
                        a3 = rd.Next(t9, t10);
                        a2 = rd.Next(t11, t12);
                    }
                }
                else if (p == 2)
                {
                    a2 = rd.Next(t7, t8);
                    int f = rd.Next(1, 3);
                    if (f == 1)
                    {
                        a1 = rd.Next(t9, t10);
                        a3 = rd.Next(t11, t12);
                    }
                    else
                    {
                        a3 = rd.Next(t9, t10);
                        a1 = rd.Next(t11, t12);
                    }
                }
                else
                {
                    a3 = rd.Next(t7, t8);
                    int f = rd.Next(1, 3);
                    if (f == 1)
                    {
                        a1 = rd.Next(t9, t10);
                        a2 = rd.Next(t11, t12);
                    }
                    else
                    {
                        a2 = rd.Next(t9, t10);
                        a1 = rd.Next(t11, t12);
                    }
                }
            }

            Thread.Sleep(100);
        }
        private async void createFox(object otid)
        {
            InitWeb3();
            string s = "";
            int tid = Convert.ToInt32(otid);
            int tokenid;
            FoxModel.tFoxBase local;
            if (tid != 0)
            {

                local = new tFoxBase(tid.ToString());
            }
            else
            {
                local = new tFoxBase();
                local.GetLastModel();
            }


            if (local != null && local.txhash != "" && local.txhash != null)
            {
                s = local.txhash;
                tokenid = int.Parse(local.tokenid);
                if (tokenid != 0)
                {
                    TransactionReceipt result = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(s);
                    SetMyTextBoxValue("开始检查 tokenid:" + tokenid + " 生成状态 !" + "-" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                    while (result == null)
                    {
                        result = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(s);
                    }
                    List<Log> logs = new List<Log>();

                    var jarray = result.Logs;
                    string js = JsonConvert.SerializeObject(jarray);
                    logs = JsonConvert.DeserializeObject<List<Log>>(js);


                    if (logs.Count > 1)
                    {
                        for (int i = 0; i < logs.Count; i++)
                        {
                            if (logs[i].topics[0] == "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef")
                            {

                                int ntokenid = int.Parse(logs[i].topics[3].Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                                if (tokenid == ntokenid)
                                {
                                    ///如何上一个成功 
                                    ///写下一个
                                    local = new tFoxBase(tid.ToString());
                                    local.sta = 1;
                                    local.Update();
                                    SetMyTextBoxValue("tokenid:" + tokenid + " 生成成功!" + "-" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                                    //写入下一个
                                    tokenid++;


                                    string[] sgens = sss();
                                    string str = "";
                                    long[] gens = new long[6];
                                    for (int j = 0; j < sgens.Length; j++)
                                    {
                                        if (sgens[j] != "")
                                        {
                                            str += sgens[j] + ",";
                                            gens[j] = long.Parse(sgens[j]);
                                        }

                                    }



                                    try
                                    {
                                        var t = await foxCore.CreatePromoFoxAsync(senderAddress, gens, FoxConfig.CEO.Address, gas); //写下一个
                                        local = new tFoxBase();
                                        local.tokenid = tokenid.ToString();
                                        local.txhash = t;
                                        local.genes = str;
                                        local.sta = 0;

                                        local.Add();
                                        SetMyTextBoxValue("tokenid:" + tokenid + " 开始生成.......txHash为:" + t + "时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");

                                        createFox(tokenid);


                                    }
                                    catch (Exception ex)
                                    {
                                        string path = "9807.wav";//.wav音频文件路径
                                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
                                        createFox(0);
                                        player.PlaySync();//另起线程播放
                                        SetMyTextBoxValue("tokenid:" + tokenid + "Exception Error :" + ex.Message);
                                    }
                                }
                                else
                                {
                                    string path = "9807.wav";//.wav音频文件路径
                                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);

                                    player.PlaySync();//另起线程播放
                                    SetMyTextBoxValue("tokenid:" + tokenid + " Log Token != Old Token :" + logs[i].data);
                                    createFox(0);
                                }

                            }
                        }

                    }
                }

            }
            else
            {

                var all = await foxCore.TotalSupplyAsyncCall();
                tokenid = (int)all + 1;
                string[] sgens = sss();
                long[] gens = new long[6];
                string gstr = "";
                for (int j = 0; j < sgens.Length; j++)
                {
                    if (sgens[j] != "")
                    {

                        gens[j] = long.Parse(sgens[j]);
                        gstr += gens[j] + ",";
                    }

                }

                try
                {
                    var t = await foxCore.CreatePromoFoxAsync(senderAddress, gens, FoxConfig.CEO.Address, gas); //写下一个
                    local = new tFoxBase();
                    local.tokenid = tokenid.ToString();
                    local.txhash = t;
                    local.genes = gstr;
                    local.sta = 0;

                    local.Add();
                    SetMyTextBoxValue("tokenid:" + tokenid + " 开始生成.......txHash为:" + t + "时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");

                    createFox(tokenid);







                }
                catch (Exception ex)
                {
                    string path = "9807.wav";//.wav音频文件路径
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);

                    player.PlaySync();//另起线程播放
                    SetMyTextBoxValue("tokenid:" + tokenid + "Exception Error :" + ex.Message);
                    createFox(0);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.button3.Enabled = false;
            objThread = new Thread(new ThreadStart(Create1Fox));
            objThread.IsBackground = false;
            objThread.Start();

        }
        private async void Create1Fox()
        {
            InitWeb3();
            for (BigInteger i = 1977; i < 2006; i = i + 2)
            {
                try
                {


                    FoxModel.tBreed1 br = new tBreed1();
                    br.GetModel((int)i, (int)(i + 1));
                    var s = await foxCore.GetFoxAsyncCall(i);
                    if (s == null)
                    {
                        s = await foxCore.GetFoxAsyncCall(i);
                    }
                    if (!s.IsGestating)
                    {

                        if (s.IsReady)
                        {


                            if (br != null || br.id != 0)
                            {
                                var hash = await foxCore.BreedWithAsync(FoxConfig.CEO.Address, i, i + 1, gas, gasPrice);
                                br.birthtime = DateTime.Now;
                                br.mid = (int)i;
                                br.fid = (int)i + 1;
                                br.txhash = hash;
                                int a = br.Add();
                                //TransactionReceipt result = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(hash);
                                //      while (result == null)
                                //      {
                                //          result = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(hash);
                                //      }

                                if (a > 0)
                                {


                                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + "和" + (i + 1).ToString() + "交配成功!" + hash + "\r\n");

                                }
                                else
                                {
                                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + "和" + (i + 1).ToString() + "交配失败!" + hash + "\r\n");
                                }
                            }
                            else { SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + "已经存在" + s.IsGestating + "/" + s.IsReady + "\r\n"); }
                        }
                        else
                        {
                            SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + "没准备好" + s.IsGestating + "/" + s.IsReady + "\r\n");
                        }
                    }
                    else
                    {
                        SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + "已经怀孕" + s.IsGestating + "/" + s.IsReady + "\r\n");
                    }

                }
                catch (Exception ex)
                {

                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + "和" + (i + 1).ToString() + "交配失败!" + ex.Message + "\r\n");
                }
                Thread.Sleep(new Random().Next(4000, 5000));
            }
            MessageBox.Show("done!");
        }
        //流浪
        private void button4_Click(object sender, EventArgs e)
        {
            //统一有行囊的全出去 判断 tTravelLog sta=1的

            //tTravelLog sta改为2 (出去了) 3有消息  4  回来了
            //写到 TfoxTravelLog里       //sta 0,出去了, 1
            this.button4.Enabled = false;
            objThread = new Thread(new ThreadStart(setTravel1));
            objThread.IsBackground = false;
            objThread.Start();

        }
        void setTravel()
        {

            DataTable dt = UJia.DAL.DbHelperSQL.Query("select SUM(CAST (eth as bigint)) as eth ,min(tokenid) as tokenid ,min(ethaddress) as ethaddress  from tTrvalLog where sta=1  group by tokenid").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sql = "update tTrvalLog set sta=2 where tokenid=" + dt.Rows[i]["tokenid"].ToString() + " and sta=1";
                //update sta 
                //insertinto tra
                tFoxTravelLog tlog = new tFoxTravelLog();
                tlog.outEth = long.Parse(dt.Rows[i]["eth"].ToString());
                tlog.tokenid = dt.Rows[i]["tokenid"].ToString();
                tlog.addtime = DateTime.Now;
                tlog.ethaddress = dt.Rows[i]["ethaddress"].ToString();
                tlog.sta = 1;
                int t = tlog.Add();
                //插入中间表
                if (t > 0)
                {
                    UJia.DAL.DbHelperSQL.ExecuteSql(sql);
                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "] tokenid=" + dt.Rows[i]["tokenid"].ToString() + " 成功走了!\r\n");
                }



            }
        }

        void setTravel1()
        {
            tTrvalLog log = new tTrvalLog();
            List<tTrvalLog> list = log.GetModelList("   sta=1 ");
            List<string> tokenids = new List<string>();

            tokenids = list.Select(p => p.tokenid).Distinct().ToList();
            foreach (var item in tokenids)
            {
                var logList = list.FindAll(p => p.tokenid == item);
                long eth = logList.Sum(p => long.Parse(p.eth));
                tFoxTravelLog tlog = new tFoxTravelLog();
                tlog.outEth = eth;
                tlog.tokenid = item;
                tlog.addtime = DateTime.Now;
                tlog.ethaddress = logList[0].ethaddress;
                tlog.sta = 1;
                int travelid = tlog.Add();
                string sql = "update tTrvalLog set sta=2 where tokenid=" + item + " and sta=1";
                UJia.DAL.DbHelperSQL.ExecuteSql(sql);
                foreach (var midtable in logList)
                {
                    FoxModel.tTravelId mid = new tTravelId();
                    mid.logid = midtable.id;
                    mid.travelid = travelid;
                    mid.Add();
                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "] tokenid=" + midtable.tokenid + " 成功走了!\r\n");
                }
            }
            MessageBox.Show("ok");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.button5.Enabled = false;
            objThread = new Thread(new ThreadStart(CheckBreed));
            objThread.IsBackground = false;
            objThread.Start();
        }

        async void CheckBreed()
        {
            InitWeb3();
            FoxModel.tBreed1 breed = new tBreed1();
            SetMyTextBoxValue("[" + DateTime.Now.ToString("HH:mm:ss") + "]开始检查生产!\r\n");
            for (BigInteger i = 1551; i < 2000; i++)
            {


                var result = await foxCore.GetFoxAsyncCall(i);
                if (result.IsGestating && result.IsReady)
                {

                    SetMyTextBoxValue("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + i + "可生产!\r\n");
                }
                else if (result.IsGestating && !result.IsReady)
                {
                    SetMyTextBoxValue("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + i + "生产时间未到!\r\n");
                }
                else {   }


            }
            SetMyTextBoxValue("[" + DateTime.Now.ToString("HH:mm:ss") + "]结束检查生产!\r\n");
        }

        /// <summary>
        /// 生一代
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            this.button6.Enabled = false;
            objThread = new Thread(new ThreadStart(CheckBreedStatus));
            objThread.IsBackground = false;
            objThread.Start();
        }
        async void CheckBreedStatus()
        {
            InitWeb3();
            for (int i = 1551; i < 2000;  i++)
            {
                try
                {
                    var s = await foxCore.GetFoxAsyncCall(new BigInteger(i));
                    if (s == null)
                    {
                        s = await foxCore.GetFoxAsyncCall(new BigInteger(i));
                    }
                    if (s.IsGestating && s.IsReady)
                    {
                        SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]id=" + i + "怀孕" + s.IsGestating + ",准备好" + s.IsReady + ",生了\r\n");
                    }
                
 
                }
                catch (Exception ex)
                {

                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]id=" + i + ",Error:" + ex.Message + "\r\n");
                }

            }

            MessageBox.Show("done!");
        }
        async void CheckBreedStatusAndBirth()
        {
            InitWeb3();
            for (int i = 1707; i < 2000; i = i + 2)
            {
                try
                {
                    var s = await foxCore.GetFoxAsyncCall(new BigInteger(i));
                    if (s == null)
                    {
                        s = await foxCore.GetFoxAsyncCall(new BigInteger(i));
                    }
                    if (s.IsGestating && s.IsReady)
                    {
                        FoxModel.tCreate1Fox fox = new tCreate1Fox();
                        fox.mid = i;
                        fox.fid = (int)s.SiringWithId;
                        fox.addtime = DateTime.Now;
                        var rel = await foxCore.GiveBirthAsync(FoxConfig.CEO.Address, i, gas, gasPrice);
                        fox.txhash = rel;
                        fox.Add();
                        SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]id=" + i + "怀孕" + s.IsGestating + ",准备好" + s.IsReady + ",生了\r\n");
                    }
                    Thread.Sleep(new Random().Next(15000, 25000));
                }
                catch (Exception ex)
                {

                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]id=" + i + ",Error:" + ex.Message + "\r\n");
                }

            }

            MessageBox.Show("done!");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.button7.Enabled = false;
            objThread = new Thread(new ThreadStart(CheckBirthStatus));
            objThread.IsBackground = false;
            objThread.Start();
        }
        async void CheckBirthStatus()
        {
            InitWeb3();
            FoxModel.tCreate1Fox fox = new tCreate1Fox();
            DataTable dt = fox.GetList(" sta=0").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string hash = dt.Rows[i]["txhash"].ToString();

                TransactionReceipt result = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(hash);
                if (result == null)
                {

                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + dt.Rows[i]["mid"].ToString() + "和" + dt.Rows[i]["fid"].ToString() + "交配pedding!\r\n");
                }
                else
                {

                    if (result.Status.HexValue == "0x1")
                    {
                        fox = new tCreate1Fox();
                        fox.GetModel(Convert.ToInt32(dt.Rows[i]["id"]));
                        fox.sta = 1;
                        fox.Update();
                        SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + dt.Rows[i]["mid"].ToString() + "和" + dt.Rows[i]["fid"].ToString() + "交配OK!\r\n");


                    }
                    else
                    {
                        fox = new tCreate1Fox();
                        fox.Delete(Convert.ToInt32(dt.Rows[i]["id"]));
                        SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + dt.Rows[i]["mid"].ToString() + "和" + dt.Rows[i]["fid"].ToString() + "交配Error!\r\n");
                    }
                }
            }
            MessageBox.Show("done!");

        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.button8.Enabled = false;
            objThread = new Thread(new ThreadStart(CheckBrith));
            objThread.IsBackground = false;
            objThread.Start();
        }
        async void CheckBrith()
        {
            try
            {
                InitWeb3();
                tBirthLog birth = new tBirthLog();
                DataTable dt = birth.GetList(" sta=0   ").Tables[0];
                int all = dt.Rows.Count;
                SetMyTextBoxValue("共有" + all + "笔出生未确认!" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                for (int i = 0; i < all; i++)
                {
                    string s = dt.Rows[i]["txhash"].ToString();
                    SetMyTextBoxValue("开始检查出生 hash为:" + s + "==>" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");

                    TransactionReceipt result = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(s.Trim());

                    try
                    {
                        if (result == null)
                        {

                        }
                        else
                        {
                            if (result.Status.HexValue == "0x1")
                            {

                                birth = new tBirthLog();
                                birth.GetModelByhash(s);
                                birth.sta = 1;
                                birth.Update();


                                tTranLog log = new tTranLog();
                                log.GetModelByhash(s);
                                log.sta = 1;
                                log.Update();
                                //添加Fox?
                                tFox fox = new FoxModel.tFox();
                                LogsModel logs = JsonConvert.DeserializeObject<LogsModel>(result.Logs[1].ToString());
                                fox.tokenid = Convert.ToInt32(logs.log.topics[3].ToString().Replace("0x", ""), 10);
                                fox.owneraddress = birth.owneraddress;
                                fox.txhash = s;
                                fox.birthTime = (ulong)Tools.GetTimeSpan(DateTime.Now);
                                fox.matronId = int.Parse(birth.matronId);
                                fox.sireId = int.Parse(birth.sireId);
                                fox.sta = 1;
                                fox.Add();
                                SetMyTextBoxValue("出生hash为:" + s + "成功==>" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                            }
                            else
                            {
                                birth = new tBirthLog();
                                birth.GetModelByhash(s);
                                birth.sta = -1;
                                birth.Update();


                                tTranLog log = new tTranLog();
                                log.GetModelByhash(s);
                                log.sta = -1;
                                log.Update();
                                SetMyTextBoxValue("出生hash为:" + s + "失败==>" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        SetMyTextBoxValue("出生hash为:" + s + "错误==>" + ex.Message + "-" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                    }


                }
                UJia.Common.LogHelper.Info(null, "出生刷新完毕,共成功:" + all + " 笔" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                Thread.Sleep(1000);//服务间隔时间
                CheckBrith();


            }
            catch (Exception ex)
            {
                UJia.Common.LogHelper.Error(null, ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                Thread.Sleep(1000);//服务间隔时间
                CheckBrith();

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.button9.Enabled = false;
            objThread = new Thread(new ThreadStart(GetGox));
            objThread.IsBackground = false;
            objThread.Start();
        }
        async void GetGox1()
        {
            string add = "0xaafAE9ea6f92c26cf35feB2c03471240A1dC1bDa";
            InitWeb3();
            FoxCoreService fox = new FoxCoreService(web3, FoxConfig.CoreAddress);
            var s = await fox.BalanceOfAsyncCall(add);

            while (s == null)
            {
                s = await fox.BalanceOfAsyncCall(add);
            }
            SetMyTextBoxValue("共" + s + "只");
            List<BigInteger> foxes = new List<BigInteger>();
            for (int i = 200; i < s; i++)
            {
                var _foxId = await fox.TokensOfOwnerByIndexAsyncCall(add, i);
                while (_foxId == 0)
                {
                    _foxId = await fox.TokensOfOwnerByIndexAsyncCall(add, i);

                }

                var foxss = await fox.GetFoxAsyncCall(_foxId);

                SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + _foxId + ":"+ foxss.Generation+"代\r\n");
             



            }
            SetMyTextBoxValue("ok!");

        }

        async void GetGox()
        {
            string add = "0xc255bdae3548631bd659da8df095dc6ff75a7427";
            InitWeb3();
            FoxCoreService fox = new FoxCoreService(web3, FoxConfig.CoreAddress);
            var s = await fox.BalanceOfAsyncCall(add);

            while (s == null)
            {
                s = await fox.BalanceOfAsyncCall(add);
            }
            SetMyTextBoxValue("共" + s + "只");
           List<BigInteger> foxes = new List<BigInteger>();
            for (int i = 0; i < s; i++)
            {
                try
                {
                    var _foxId = await fox.TokensOfOwnerByIndexAsyncCall(add, i);
                    while (_foxId == 0)
                    {
                        _foxId = await fox.TokensOfOwnerByIndexAsyncCall(add, i);

                    }
                    tSale sale = new tSale();
                    tFox foxuser = new tFox();
                    tFoxBase foxs = new tFoxBase();
                    var foxss = await fox.GetFoxAsyncCall(_foxId);
                    var sfox = await foxSiring.GetAuctionAsyncCall(_foxId);
                    var bfox = await foxCore.GetFoxAsyncCall(_foxId);
                    sale.GetModel(_foxId.ToString());
                    if (sale == null || sale.id == 0)
                    {
                        sale.addtime = Tools.TimeSpanToDateTime((long)sfox.StartedAt);
                        sale.birthday = bfox.BirthTime.ToString();
                        sale.endprice = sfox.EndingPrice.ToString();
                        sale.txhash = "--";
                        sale.tokenid = _foxId.ToString();
                        sale.sta = 1;
                        sale.owners = sfox.Seller;
                        sale.startprice = sfox.StartingPrice.ToString();
                        sale.stype = 2;
                        sale.geneation = bfox.Generation.ToString();
                        sale.endtime = sale.addtime.AddSeconds((double)sfox.Duration);
                        try
                        {
                            sale.Add();
                            //   foxes.Add(_foxId);
                            SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + _foxId + "Sale:ok");
                        }
                        catch (Exception ex)
                        {


                            //   foxes.Add(_foxId);
                            SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + _foxId + ":error:" + ex.Message);
                        }
                    }
                }
                catch (Exception e)
                {

                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]error:" + e.Message);
                }
            
                //foxuser.GetModel(_foxId.ToString());
                //foxs.GetModel(_foxId.ToString());
                //var saller = sfox.Seller;

                //if (foxs.tokenid == "" || foxs == null || foxs.tokenid == "0" || foxs.tokenid == null)
                //{
                //    foxs = new FoxModel.tFoxBase();
                //    foxs.tokenid = _foxId.ToString();
                //    foxs.birthTime = (long)bfox.BirthTime;
                //    foxs.sta = -1;
                //    foxs.birthTime = (long)bfox.BirthTime;
                //    foxs.siringWithId = (long)bfox.SiringWithId;
                //    foxs.sireId = (long)bfox.SireId;
                //    foxs.nextActionAt = (long)bfox.NextActionAt;
                //    foxs.matronId = (long)bfox.MatronId;
                //    foxs.isReady = bfox.IsReady;
                //    foxs.isGestating = bfox.IsGestating;
                //    string str = "";
                //    for (int k = 0; k < bfox.Genes.Count; k++)
                //    {
                //        str += bfox.Genes[k] + ",";
                //    }
                //    foxs.genes = str;
                //    foxs.generation = (long)bfox.Generation;
                //    foxs.cooldownIndex = (long)bfox.CooldownIndex;



                //    FoxModel.tFeedLog feed = new FoxModel.tFeedLog();
                //    string sql = "select sum(cast(eth as bigint)) as s from tFeedLog where sta=1 and tokenid='" + _foxId + "'";
                //    string eth = Convert.ToString((DbHelperSQL.GetSingle(sql)));

                //    string c = "";
                //    if (eth != "")
                //    {
                //        BigInteger mi = BigInteger.Parse(eth);
                //        var grow = bfox.Genes[3] / 1000000000000 % 1000;

                //        grow = new FoxCtrl().GetFoxAttrScoure((int)_foxId, 1, (int)grow);
                //        decimal mi2 = ((decimal)mi / 10000000000000000) * (decimal)grow;
                //        c = mi2.ToString();
                //    }
                //    else
                //    {
                //        c = "0";
                //    }
                //    foxs.charm = Convert.ToDecimal(c);
                //    foxs.Add();
                //    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + _foxId + "FoxBase:done");
                //}
                //if (foxuser == null || foxuser.tokenid == 0)
                //{
                //    foxuser = new tFox();
                //    foxuser.tokenid = (int)_foxId;
                //    foxuser.owneraddress = saller;
                //    foxuser.Add();
                //    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + _foxId + "FoxUser:done");
                //}
          

             
            }
            SetMyTextBoxValue("ok!");

        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.button10.Enabled = false;
            objThread = new Thread(new ThreadStart(GetRefish));
            objThread.IsBackground = false;
            objThread.Start();
            Thread t2= new Thread(new ThreadStart(GetRefish1));
            t2.Start();
            Thread t3 = new Thread(new ThreadStart(GetRefish2));
            t3.Start();
        }
        async void GetRefish()
        {
            SetMyTextBoxValue("1[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]begin");
            InitWeb3();
            for (BigInteger i =2001 ; i <= 2200; i++)
            {
                try
                {
                    var fox = await foxCore.GetFoxAsyncCall(i);
                tFoxBase foxbase = new tFoxBase();
                tFox foxo = new tFox();
                foxbase.tokenid = i.ToString();
                foxbase.birthTime = (long)fox.BirthTime;
                foxbase.charm = 0;
                foxbase.cooldownIndex = (long)fox.CooldownIndex;
                foxbase.generation = (long)fox.Generation;
                string gs = "";
                for (int j = 0; j < fox.Genes.Count; j++)
                {
                    gs += fox.Genes[j]+",";
                }
                foxo.genes = gs;
                foxo.generation = (int)foxbase.generation;
                foxbase.genes =gs ;
                foxbase.isGestating = fox.IsGestating;
                foxbase.isReady = fox.IsReady;
                foxbase.matronId = (long)fox.MatronId;
                foxbase.sireId = (long)fox.SireId;
                foxbase.siringWithId = (long)fox.SiringWithId;
                foxbase.sta = 0;
                foxbase.txhash = "--";
                var owner = await foxCore.OwnerOfAsyncCall(i);
                foxo.tokenid = (int)i;
                foxo.owneraddress = owner;
               
                    foxbase.Add();
                    foxo.Add();

                    SetMyTextBoxValue("1[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + ":ok");
                }
                catch (Exception ex)
                {
                  

                    SetMyTextBoxValue("1[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + ":error:"+ex.Message);
               
                }
             await   Task.Delay(1000);
            }
            SetMyTextBoxValue("1[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]end");

        }
        async void GetRefish1()
        {
            SetMyTextBoxValue("2[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]begin");
            InitWeb3();
            for (BigInteger i = 2201; i <= 2400; i++)
            {
                try
                {
                    var fox = await foxCore.GetFoxAsyncCall(i);
                    tFoxBase foxbase = new tFoxBase();
                    tFox foxo = new tFox();
                    foxbase.tokenid = i.ToString();
                    foxbase.birthTime = (long)fox.BirthTime;
                    foxbase.charm = 0;
                    foxbase.cooldownIndex = (long)fox.CooldownIndex;
                    foxbase.generation = (long)fox.Generation;
                    string gs = "";
                    for (int j = 0; j < fox.Genes.Count; j++)
                    {
                        gs += fox.Genes[j] + ",";
                    }
                    foxo.genes = gs;
                    foxo.generation = (int)foxbase.generation;
                    foxbase.genes = gs;
                    foxbase.isGestating = fox.IsGestating;
                    foxbase.isReady = fox.IsReady;
                    foxbase.matronId = (long)fox.MatronId;
                    foxbase.sireId = (long)fox.SireId;
                    foxbase.siringWithId = (long)fox.SiringWithId;
                    foxbase.sta = 0;
                    foxbase.txhash = "--";
                    var owner = await foxCore.OwnerOfAsyncCall(i);
                    foxo.tokenid = (int)i;
                    foxo.owneraddress = owner;

                    foxbase.Add();
                    foxo.Add();

                    SetMyTextBoxValue("2[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + ":ok");
                }
                catch (Exception ex)
                {


                    SetMyTextBoxValue("2[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + ":error:" + ex.Message);

                }
                await Task.Delay(1000);
            }
            SetMyTextBoxValue("2[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]end");
        }

        async void GetRefish2()
        {
            SetMyTextBoxValue("3[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]begin");
            InitWeb3();
            for (BigInteger i = 2401; i <= 2600; i++)
            {
                try
                {
                    var fox = await foxCore.GetFoxAsyncCall(i);
                    tFoxBase foxbase = new tFoxBase();
                    tFox foxo = new tFox();
                    foxbase.tokenid = i.ToString();
                    foxbase.birthTime = (long)fox.BirthTime;
                    foxbase.charm = 0;
                    foxbase.cooldownIndex = (long)fox.CooldownIndex;
                    foxbase.generation = (long)fox.Generation;
                    string gs = "";
                    for (int j = 0; j < fox.Genes.Count; j++)
                    {
                        gs += fox.Genes[j] + ",";
                    }
                    foxo.genes = gs;
                    foxo.generation = (int)foxbase.generation;
                    foxbase.genes = gs;
                    foxbase.isGestating = fox.IsGestating;
                    foxbase.isReady = fox.IsReady;
                    foxbase.matronId = (long)fox.MatronId;
                    foxbase.sireId = (long)fox.SireId;
                    foxbase.siringWithId = (long)fox.SiringWithId;
                    foxbase.sta = 0;
                    foxbase.txhash = "--";
                    var owner = await foxCore.OwnerOfAsyncCall(i);
                    foxo.tokenid = (int)i;
                    foxo.owneraddress = owner;

                    foxbase.Add();
                    foxo.Add();

                    SetMyTextBoxValue("3[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + ":ok");
                }
                catch (Exception ex)
                {


                    SetMyTextBoxValue("3[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + i + ":error:" + ex.Message);

                }
                await Task.Delay(1000);
            }
            SetMyTextBoxValue("3[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]end");
        }
        void Insertid()
        {
            FoxModel.tFoxTravelLog travle = new tFoxTravelLog();
            DataTable dt = travle.GetList(" 1=1 ").Tables[0];
            tTravelId midtable = new tTravelId();
            int a;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tTrvalLog log = new tTrvalLog();
                DataTable dt1 = new DataTable();
                dt1 = log.GetList(" tokenid=" + dt.Rows[i]["tokenid"].ToString()).Tables[0];
                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    midtable = new tTravelId();
                    midtable.logid = int.Parse(dt1.Rows[j]["id"].ToString());
                    midtable.travelid = int.Parse(dt.Rows[i]["id"].ToString());
                    a = midtable.Add();
                    SetMyTextBoxValue("[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]" + a);
                }
            }
            SetMyTextBoxValue("ok!");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.button11.Enabled = false;
            objThread = new Thread(new ThreadStart(CreateResult));
            objThread.IsBackground = false;
            objThread.Start();
        }

         void CreateResult()
        {
            int[] step1 = { 1, 2, 3, 4, 5,6 };
            int[] step2 = { 7, 8, 9, 10 ,11,12};
            int[] step3 = { 13, 14, 15,16,17,18 };
            tFoxTravelLog tlog = new tFoxTravelLog();
            var trvalList = tlog.GetModelList("sta=1");
            long totalEth = 0;//总ETH
            long totalLucky = 0;//总运气值
            long avgLucky = 0;//平均运气值
            decimal toalPercent = 0;//总暴击系数
            decimal totalUValue = 0;//总U值
            decimal totleSeries = 0;//总系数
            List<foxTrval> foxList = new List<foxTrval>();
            if (trvalList != null && trvalList.Count > 0)
            {
                totalEth = trvalList.Sum(p =>p.outEth);

                tFoxBase fox = new tFoxBase();
                foreach (var item in trvalList)
                {
                    fox.GetModel(item.tokenid.ToString());
                    string[] gensArr = fox.genes.Split(',');
                    long lucky = long.Parse(gensArr[3]) / 1000 % 1000;//运气值
                    long stuff = long.Parse(gensArr[2]) / 1000000000000 % 1000;//天赋
                    long engory = long.Parse(gensArr[3]) / 1000000000 % 1000;//精力
                    totalLucky += lucky;
                    
                    decimal uvalue = (decimal)item.outEth / 1000000000000;
                    totalUValue += uvalue;

                    Random rdm = new Random();
                    decimal percent = (decimal)rdm.Next(80, 120) / 100;
                    toalPercent += percent;
                    totleSeries += (uvalue + lucky) * percent;
                    foxTrval foxInfo = new foxTrval
                    {
                        tokenid = item.tokenid,
                        ethaddress = item.ethaddress,
                        travelid = item.id,
                        uvalue = uvalue,
                        stuff = stuff,
                        engory = engory,
                        currentlucky = lucky,
                        percent = percent,
                        location = "",
                        outeht = item.outEth.ToString(),
                    };
                    foxList.Add(foxInfo);
                }

         
           

                //计算平均运气值
                avgLucky = totalLucky / trvalList.Count;
                //按运气值升序排列
                foxList=foxList.OrderBy(p => p.currentlucky).ToList();
                //所有运气值大于平均运气值的狐狸
                var luckyList = foxList.FindAll(p => p.currentlucky > avgLucky);
                decimal maxRate = 0;//最大占比
                List<foxTrval> backList = new List<foxTrval>();//已回去的记录
                SetMyTextBoxValue("[总狐狸]" + trvalList.Count + "\r\n");
                SetMyTextBoxValue("[总运气值]" + totalLucky + "\r\n");
                SetMyTextBoxValue("[总ETH]" + totalEth + "\r\n");
                SetMyTextBoxValue("[总暴击系数]" + toalPercent + "\r\n");
                SetMyTextBoxValue("[总U值]" + totalUValue + "\r\n");
                SetMyTextBoxValue("[总系数]" + totleSeries + "\r\n");
                SetMyTextBoxValue("[平均运气值]" + avgLucky + "\r\n");
                SetMyTextBoxValue("[运气值大于平均]" + luckyList.Count + "\r\n");
        
                #region 第一批狐狸(大于平均值的三分之一)

                if (luckyList != null && luckyList.Count > 0)
                {
                    maxRate =(decimal)luckyList.Count / (decimal)trvalList.Count;
                    SetMyTextBoxValue("luckyList" + luckyList.Count + "\r\n");
                    SetMyTextBoxValue("trvalList" + trvalList.Count + "\r\n");
                    SetMyTextBoxValue("高于平均占比" + maxRate + "\r\n");
                    decimal halfETH = totalEth / 2;
                    SetMyTextBoxValue("halfETH:" + halfETH + "\r\n");
                    decimal realeth = 0;
                    //判断比例是否小于三分之一,带回一半ETH
                    if (maxRate <= (1 / 3))
                    {

             
                        foreach (var model in luckyList)
                        {
                            //为每只狐狸增加ETH
                            decimal eth = 0;
                            eth = (((decimal)model.currentlucky + model.uvalue) * model.percent *3/ totleSeries) * halfETH;///每个人分掉ETH
                            SetMyTextBoxValue(model.tokenid + "[<1/3拿出]" + (decimal.Parse(model.outeht) / 1000000000000000000) + "拿回" + eth + (decimal.Parse(model.outeht) > eth ? "{亏}" : "") + "运气值" + model.currentlucky + "爆机" + model.percent + "U值" + model.uvalue + "\r\n");

                            realeth += eth;
                            //  -----------------------------   //如果亏了送一个 低级宝石

                            ////更新travellog 和tfoxfravellog
                            FoxModel.tTrvalLog trvalLog = new tTrvalLog();
                            trvalLog.Update(model.tokenid, 4);

                            tFoxTravelLog flog = new tFoxTravelLog();
                            flog.Update(model.tokenid, 8, 100, DateTime.Now);

                            ////带回去写reslult表----------------
                            FoxModel.tTravelResult resule = new tTravelResult();
                            resule.intype = 100;//挖eth
                            resule.jueryid = 0;
                            resule.juerycount = (long)eth; //eth数量
                            resule.sta = 6;
                            resule.travelid = model.travelid;
                            resule.tokenid = long.Parse(model.tokenid);
                            resule.Add();
                            //写t收益表 intype=1
                            trecive rec = new trecive();
                            rec.allmoney = "0";
                            rec.lefteth = "0";
                            rec.thischarm = 0;
                            rec.thiseth = eth.ToString();
                            rec.thispersnt = model.percent;
                            rec.tokenid = model.tokenid;
                            rec.addtime = DateTime.Now;
                            rec.sta = 0;
                            rec.intype = 1;
                            rec.Add();
                        }
                        backList = luckyList;
                    }
                    else// if (maxRate > (1 / 3)&&maxRate<(1/2))
                    {
                        SetMyTextBoxValue("[------运气值大于平均]大于1/3\r\n");
                        //运气值升序排列,前总数的三分之一回去
                        var tempList = luckyList.OrderBy(p => p.currentlucky);

                        int topcount = foxList.Count / 3;//计算回去的狐狸数

                        backList = tempList.Skip((1 - 1) * topcount).Take(topcount).ToList();
                        SetMyTextBoxValue("[拿eth回去的有]"+ backList .Count+ "人\r\n");
                        foreach (var model in backList)
                        {
                            decimal eth = 0;
                            eth = (((decimal)model.currentlucky + model.uvalue) * model.percent*3/ totleSeries) * halfETH;///每个人分掉ETH
                            realeth += eth;
                            SetMyTextBoxValue(model.tokenid + "[<1/3拿出]" + (decimal.Parse(model.outeht) / 1000000000000000000) + "拿回" + (eth/ 1000000000000000000) + (decimal.Parse(model.outeht) > eth?"{亏}":"" ) + "运气值" + model.currentlucky + "爆机" + model.percent + "U值" + model.uvalue + "\r\n");

                            ////更新travellog 和tfoxfravellog
                            tTrvalLog trvalLog = new tTrvalLog();
                            trvalLog.Update(model.tokenid, 4);

                            tFoxTravelLog flog = new tFoxTravelLog();
                            flog.Update(model.tokenid, 8, 100, DateTime.Now);

                            ////带回去写reslult表----------------
                            FoxModel.tTravelResult resule = new tTravelResult();
                            resule.intype = 100;//挖eth
                            resule.jueryid = 0;
                            resule.juerycount = (long)eth; //eth数量
                            resule.sta = 6;
                            resule.travelid = model.travelid;
                            resule.tokenid = long.Parse(model.tokenid);
                            resule.Add();


                            //写t收益表 intype=1
                            trecive rec = new trecive();
                            rec.allmoney = "0";
                            rec.lefteth = "0";
                            rec.thischarm = 0;
                            rec.thiseth = eth.ToString();
                            rec.thispersnt = model.percent;
                            rec.tokenid = model.tokenid;
                            rec.addtime = DateTime.Now;
                            rec.sta = 0;
                            rec.intype = 1;
                            rec.Add();
                        }

                        SetMyTextBoxValue("[    realeth  ]" + realeth + " \r\n");
                        //大于平均值排除已回去的狐狸
                        SetMyTextBoxValue("[第一批 共 ]" + backList.Count + "人\r\n");

                        List<string> tokenIdList = backList.Select(o => o.tokenid).ToList();   
                        var remainingList = luckyList.FindAll(p => tokenIdList.Contains(p.tokenid)==false);
                        SetMyTextBoxValue("[第一批剩余有]"+ remainingList .Count+ "人\r\n");
                        int number = 0;
       
                        foreach (var model in remainingList)
                        {
            
                            #region 发放中等宝石

                            List<int> juary = new List<int>();
                            for (int i = 0; i < remainingList.Count; i++)
                            {
                                Random rm = new Random();
                                int num = rm.Next(0, step2.Length - 1);
                                Thread.Sleep(10);
                                juary.Add(step2[num]);
                            }

                            SetMyTextBoxValue("["+DateTime.Now+"]"+model.tokenid + "[拿出]" + (decimal.Parse(model.outeht) / 1000000000000000000) + "拿回中等宝石" + juary[number] + "\r\n");
                            //////更新travellog 和tfoxfravellog
                            tTrvalLog trvalLog = new tTrvalLog();
                            trvalLog.Update(model.tokenid, 4);

                            tFoxTravelLog flog = new tFoxTravelLog();
                            flog.Update(model.tokenid, 8, 200, DateTime.Now);

                            ////带回去写reslult表----------------
                            tTravelResult resule = new tTravelResult();
                            resule.intype = 200;//挖宝
                            resule.jueryid = juary[number];
                            int jcount = new Random().Next(1, 2);
                            resule.juerycount = jcount;
                            resule.sta = 6;
                            resule.travelid = model.travelid;
                            resule.tokenid = long.Parse(model.tokenid);
                            resule.Add();

                            //写用户宝石表 tUserJuery  宝石记录tUserJueryLog
                            tUserJueryLog ulog = new tUserJueryLog();
                            ulog.tokenid = long.Parse(model.tokenid);
                            ulog.ethaddress = model.ethaddress;
                            ulog.addtime = DateTime.Now;
                            ulog.Jueryid = juary[number];
                            ulog.usecount = jcount;
                            ulog.Add();
                            tUserJuery uje = new tUserJuery();
                            uje.GetModel(juary[number], model.ethaddress);
                            if (uje != null && uje.id != 0)
                            {
                                uje.JHerycount = uje.JHerycount + jcount;
                                uje.Update();
                            }
                            else
                            {
                                uje.ethaddress = model.ethaddress;
                                uje.Jueryid = juary[number];
                                uje.JHerycount = jcount;
                                uje.Add();
                            }
                            number++;
                            #endregion
                        }
                    }
                   
                }
                #endregion

                #region 第二批狐狸
                //排除第一批的狐狸
                var foxList2 = foxList.FindAll(p => luckyList.Select(o => o.tokenid).Contains(p.tokenid) == false);
                SetMyTextBoxValue("[第二批]" + foxList2.Count + "\r\n");
                if (foxList2 != null && foxList2.Count > 0)
                {
      

                    int juerycount = (int)Math.Ceiling(foxList2.Count  * 1.1);//分宝物总数

                    #region 计算宝物数
                    int firstCount = 0;
                    int secondCount = 0;
                    int thirdCount = 0;
              
                    firstCount = (int)Math.Ceiling(Convert.ToDecimal(juerycount * 7 / 10));
                    secondCount = (int)Math.Ceiling(Convert.ToDecimal(juerycount * 2 / 10));
                    thirdCount = (int)Math.Ceiling(Convert.ToDecimal(juerycount * 1 / 10));

                    List<int> juary = new List<int>();
                    for (int i = 0; i < firstCount; i++)
                    {
                        Random rm = new Random();
                        int num = rm.Next(0, step1.Length - 1);
                        juary.Add(step1[num]);
                        Thread.Sleep(10);
                    }
                    for (int i = 0; i < secondCount; i++)
                    {
                        Random rm = new Random();
                        int num = rm.Next(0, step2.Length - 1);
                        juary.Add(step2[num]);
                        Thread.Sleep(10);
                    }
                    for (int i = 0; i < thirdCount; i++)
                    {
                        Random rm = new Random();
                        int num = rm.Next(0, step3.Length - 1);
                        juary.Add(step3[num]);
                        Thread.Sleep(10);
                    }
                    #endregion
              
                    //找出要发放宝物的狐狸
                    var jureyList = foxList2; //foxList2.FindAll(p => radomList.Contains(p.tokenid));
                    //运气值*暴击系数降序
                    jureyList = jureyList.OrderByDescending(p => p.currentlucky * p.percent).ToList();
                    int number = 0;
                    if (jureyList != null && jureyList.Count > 0)
                    {
                        int leftjuary = juary.Count - jureyList.Count;
                        foreach (var model in jureyList)
                        {
                            int index = new Random().Next(0, juary.Count - 1);
                            SetMyTextBoxValue(model.tokenid + "[拿出]" + (decimal.Parse(model.outeht)/1000000000000000000) + "拿回宝石" + juary[index] + "\r\n");

                            //////更新travellog 和tfoxfravellog
                            tTrvalLog trvalLog = new tTrvalLog();
                            trvalLog.Update(model.tokenid, 4);

                            tFoxTravelLog flog = new tFoxTravelLog();
                            flog.Update(model.tokenid, 8, 200, DateTime.Now);


                            ////带回去写reslult表----------------
                            tTravelResult resule = new tTravelResult();
                            resule.intype = 200;//挖宝
                            resule.jueryid = juary[number];
                            int jcount = new Random().Next(1, 2);
                            resule.juerycount = jcount;
                            resule.sta = 6;
                            resule.travelid = model.travelid;
                            resule.tokenid = long.Parse(model.tokenid);
                            resule.Add();

                            //写用户宝石表 tUserJuery  宝石记录tUserJueryLog
                            tUserJueryLog ulog = new tUserJueryLog();
                            ulog.tokenid = long.Parse(model.tokenid);
                            ulog.ethaddress = model.ethaddress;
                            ulog.addtime = DateTime.Now;
                            ulog.Jueryid = juary[number];
                            ulog.usecount = jcount;
                            ulog.Add();
                            tUserJuery uje = new tUserJuery();
                            uje.GetModel(juary[number], model.ethaddress);
                            if (uje != null && uje.id != 0)
                            {
                                uje.JHerycount = uje.JHerycount + jcount;
                                uje.Update();
                            }
                            else
                            {
                                uje.ethaddress = model.ethaddress;
                                uje.Jueryid = juary[number];
                                uje.JHerycount = jcount;
                                uje.Add();
                            }
                            Thread.Sleep(10);
                            number++;
                        }

                    }
                }
                #endregion

                #region 第三批狐狸
                //排除第一和第二批的狐狸
                var tempfoxList= foxList.FindAll(p => luckyList.Select(o => o.tokenid).Contains(p.tokenid) == false);//排除第一批
                var foxList3 = tempfoxList.FindAll(p => foxList2.Select(o => o.tokenid).Contains(p.tokenid) == false);//排除第二批
    
                if (foxList3 != null && foxList3.Count > 0)
                {
                    List<string> tokenIdList = foxList3.Select(p => p.tokenid).ToList();

                    List<string> radomList = new List<string>();
                    int halfCount = foxList3.Count / 2;//剩余狐狸的一半

                 
                    foreach (var item in tokenIdList)
                    {
                        Random rm = new Random();
                        int i = rm.Next(tokenIdList.Count);  //随机数最大值不能超过list的总数
                        Thread.Sleep(10);                      //随机追加狐狸
                        if (radomList.Count < halfCount)
                        {
                            radomList.Add(item);
                        }
                        else
                        {
                            break;
                        }
                    }

                    //随机获取第三批狐狸的一半
                    backList = foxList3.FindAll(p => radomList.Contains(p.tokenid));
                    int number = 0;
                    SetMyTextBoxValue("[第三批一半有东西的]" + backList.Count + "\r\n");
                    foreach (var model in backList)
                    {
                       
                        #region 发放低等宝石                                  
                        List<int> juary = new List<int>();
                        for (int i = 0; i < foxList3.Count-halfCount; i++)
                        {
                            Random rm = new Random();
                            int num = rm.Next(0, step1.Length - 1);
                            juary.Add(step1[num]);
                            Thread.Sleep(10);
                        }
                        SetMyTextBoxValue(model.tokenid + "[拿出]" + model.outeht + "拿回低等宝石" + juary[number] + "\r\n");
                        //////更新travellog 和tfoxfravellog
                        tTrvalLog trvalLog = new tTrvalLog();
                        trvalLog.Update(model.tokenid, 4);

                        tFoxTravelLog flog = new tFoxTravelLog();
                        flog.Update(model.tokenid, 8, 200, DateTime.Now);


                        ////带回去写reslult表----------------
                        tTravelResult resule = new tTravelResult();
                        resule.intype = 200;//挖宝
                        resule.jueryid = juary[number];
                        int jcount = new Random().Next(1, 2);
                        resule.juerycount = jcount;
                        resule.sta = 6;
                        resule.travelid = model.travelid;
                        resule.tokenid = long.Parse(model.tokenid);
                        resule.Add();

                        //写用户宝石表 tUserJuery  宝石记录tUserJueryLog
                        tUserJueryLog ulog = new tUserJueryLog();
                        ulog.tokenid = long.Parse(model.tokenid);
                        ulog.ethaddress = model.ethaddress;
                        ulog.addtime = DateTime.Now;
                        ulog.Jueryid = juary[number];
                        ulog.usecount = jcount;
                        ulog.Add();
                        tUserJuery uje = new tUserJuery();
                        uje.GetModel(juary[number], model.ethaddress);
                        if (uje != null && uje.id != 0)
                        {
                            uje.JHerycount = uje.JHerycount + jcount;
                            uje.Update();
                        }
                        else
                        {
                            uje.ethaddress = model.ethaddress;
                            uje.Jueryid = juary[number];
                            uje.JHerycount = jcount;
                            uje.Add();
                        }
                        number++;

                        //
                        #endregion



                    }

                    //空手而归胡狐狸
                    var emptyFoxList = foxList3.FindAll(p => backList.Select(o => o.tokenid).Contains(p.tokenid) == false);
                    SetMyTextBoxValue("[第三批一半无东西的]" + emptyFoxList.Count + "\r\n");
                    foreach (var model in emptyFoxList)
                    {
                        SetMyTextBoxValue(model.tokenid + "[拿出]" + model.outeht + "空手\r\n");
                        //////更新travellog 和tfoxfravellog
                        tTrvalLog trvalLog = new tTrvalLog();
                        trvalLog.Update(model.tokenid, 4);

                        tFoxTravelLog flog = new tFoxTravelLog();
                        flog.Update(model.tokenid, 8, 200, DateTime.Now);
                        int ujid = new Random().Next(1, 6);

                        ////带回去写reslult表----------------
                        tTravelResult resule = new tTravelResult();
                        resule.intype = 200;//挖宝
                        resule.jueryid = ujid;
                        int jcount = new Random().Next(1, 2);
                        resule.juerycount = jcount;
                        resule.sta = 6;
                        resule.travelid = model.travelid;
                        resule.tokenid = long.Parse(model.tokenid);
                        resule.Add();

                        //写用户宝石表 tUserJuery  宝石记录tUserJueryLog
                        tUserJueryLog ulog = new tUserJueryLog();
                        ulog.tokenid = long.Parse(model.tokenid);
                        ulog.ethaddress = model.ethaddress;
                        ulog.addtime = DateTime.Now;
                        ulog.Jueryid = ujid;
                        ulog.usecount = jcount;
                        ulog.Add();
                        tUserJuery uje = new tUserJuery();
                        uje.GetModel(ujid, model.ethaddress);
                        if (uje != null && uje.id != 0)
                        {
                            uje.JHerycount = uje.JHerycount + jcount;
                            uje.Update();
                        }
                        else
                        {
                            uje.ethaddress = model.ethaddress;
                            uje.Jueryid = ujid;
                            uje.JHerycount = jcount;
                            uje.Add();
                        }
                        number++;

                    }

                }
                #endregion
            }
            SetMyTextBoxValue("ok");

        }
    }



}
