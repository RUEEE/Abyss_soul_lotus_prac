using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEditor;
using System.Xml.Linq;
using UnityEngine.Windows;
using HarmonyLib;
using System.Collections;
using UnityEngine.EventSystems;
using System.Globalization;
using System.IO;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using System.Collections.ObjectModel;

namespace ASL_Prac
{
    static class Prac_Variables
    {
        public static bool is_invincable = false;
        public static bool is_disable_X = false;
        public static bool is_ctrl_spdup = false;
        public static bool is_disable_Pause = false;
        public static bool is_fasterBGM = false;

        public static int gameFPS = 60;
    }
    static class Prac_Locale
    {
        static Dictionary<string, string> Locale_CN = new Dictionary<string, string>{
            { "border","结界" },
            { "border_desc","结界不满200%时过面会自动加回200%" },
            { "dian","得点" },
            { "graze","擦弹" },
            { "life","残" },
            { "lifepiece","残碎" },
            { "time","时间" },
            { "score","分数" },
            { "LSC","LSC" },

            { "Invincable","无敌" },
            { "Unlock all stage","全关解锁" },
            { "Disable X key","禁用X键" },
            { "Disable Pause","切屏不停" },
            { "Faster15f","加速15f" },
            { "Slower15f","减速15f" },
            { "FasterBGM","加速BGM" },

            { "Kill all","瞬秒敌机" },
            { "Ctrl SpdUp","Ctrl快进" },
            { "front 1"                         ,"前半 1"},
            { "front 2(after title)"            ,"前半 2(标题后)"},
            { "front 3(big butterfly)"          ,"前半 3(大蝴蝶)"},
            { "front 4(middle big butterfly)"   ,"前半 4(中间大蝴蝶)"},
            { "mid normal 1"                    ,"道中非 1"},
            { "mid spell 1"                     ,"道中符 1"},
            { "mid spell 2"                     ,"道中符 2"},
            { "later 1"                         ,"后半 1"},
            { "later 2(ju)"                     ,"后半 2(狙)"},
            { "later 3(middle big butterfly)"   ,"后半 3(中间大蝴蝶)"},
            { "front 3(red blue ju)"            ,"前半 3(红蓝狙)"},
            { "front 4(big butterfly)"          ,"前半 4(大蝴蝶)"},
            { "front 4(//\\\\)"                 ,"前半 4(掉落一坨)"},
            { "later 1(ghost bottom)"           ,"后半 1(板底鬼火)"},
            { "later 2(ghost around)"           ,"后半 2(四周鬼火)"},
            { "later 3(snow butterfly)"         ,"后半 3(雪花大蝴蝶)"},
            { "front 2(yaojing)"                ,"前半 2(妖精阵)"},
            { "front 3(circles)"                ,"前半 3(红黄甜甜圈)"},
            { "front 4(after title)"            ,"前半 4(标题后)"},
            { "front 5(2 wuke)"                 ,"前半 5(两吴克)"},
            { "front 6(<|-- ghost)"             ,"前半 6(<|--鬼火)"},
            { "front 7(feixingzhen)"            ,"前半 7(飞行阵)"},
            { "front 8(red ghost ju)"           ,"前半 8(红鬼火狙)"},
            { "front 9(black white circles)"    ,"前半 9(黑白甜甜圈)"},
            { "front 10(double zoufangdaren)"   ,"前半 10(双重诹访大人)"},
            { "front 2(red yaojing)"            ,"前半 2(红妖精)"},
            { "front 3(laser yaojing)"          ,"前半 3(激光妖精)"},
            { "front 4(horizonal laser)"        ,"前半 4(横激光阵)"},
            { "later 1(laser yaojing)"          ,"后半 1(增援激光妖精)"},
            { "later 2(laser+bullet)"           ,"后半 2(激光+子弹阵)"},
            { "later 3(laser+laser)"            ,"后半 3(激光+激光阵)"},
            { "later 4(blue mao)"               ,"后半 4(蓝猫)"},
            { "front 2(blue)"                   ,"前半 2(蓝蓝猫)"},
            { "front 3(blue ghosts)"            ,"前半 3(蓝鬼火)"},
            { "front 4(red ghosts+purple)"      ,"前半 4(红鬼火+紫蓝猫)"},
            { "front 5(blue mao)"               ,"前半 5(蓝猫)"},
            { "front 6(laser+ghosts2)"          ,"前半 6(双激光,鬼火)"},
            { "front 7(ghosts final)"           ,"前半 7(最后一坨鬼火)"},
        };
        public static string GetLocaleString(string str) {
            if(Locale_CN.ContainsKey(str)) 
                return Locale_CN[str];
            return str;
        }
    }
    class Prac_Hotkey
    {
        public delegate void HotKeyDelegate(bool val);
        public bool isActivated;
        private KeyCode hotKey;
        private string name;
        private string keyName;
        HotKeyDelegate hotKeyDelegate;
        private int wait_time = 0;
        private Color color;
        public Prac_Hotkey(KeyCode keyCode,string keyName, string name, HotKeyDelegate d)
        {
            isActivated = false;
            hotKey = keyCode;
            this.name=name;
            this.keyName = keyName;
            hotKeyDelegate = d;
            this.color = Color.white;
        }
        public Prac_Hotkey(KeyCode keyCode, string keyName, string name, HotKeyDelegate d, Color color)
        {
            isActivated = false;
            hotKey = keyCode;
            this.name=name;
            this.keyName = keyName;
            hotKeyDelegate = d;
            this.color = color;
        }
        public void OnGUI() {
           
            GUIStyle activated_style = new GUIStyle(GUI.skin.label);
            if (isActivated){
                activated_style.normal.textColor = Color.green;
                activated_style.fontStyle = FontStyle.Italic;
            }else {
                activated_style.normal.textColor = color;
            }
            GUILayout.Label($"[{keyName}] { Prac_Locale.GetLocaleString(name)}", activated_style);
            if (UnityEngine.Input.GetKeyUp(hotKey) && wait_time >= 10) {
                isActivated = !isActivated;
                hotKeyDelegate(isActivated);
                wait_time = 0;
            }
            wait_time++;
        }
    }
    static class Prac_Overlay
    {
        static private bool isOpen = false;
        static private int wait_time = 0;
        static private Rect windRc = new Rect(10, 10, 300, 400);
        static private Prac_Hotkey hotkey_invincable = new Prac_Hotkey(KeyCode.F1,"F1","Invincable",(bool s) => { Prac_Variables.is_invincable=s; });
        static private Prac_Hotkey hotkey_disable_X = new Prac_Hotkey(KeyCode.F2, "F2", "Disable X key", (bool s) => { Prac_Variables.is_disable_X = s; });
        static private Prac_Hotkey hotkey_ctrl_spdup = new Prac_Hotkey(KeyCode.F3, "F3", "Ctrl SpdUp", (bool s) => { Prac_Variables.is_ctrl_spdup = s; });
        static private Prac_Hotkey hotkey_disable_pause = new Prac_Hotkey(KeyCode.F4, "F4", "Disable Pause", (bool s) => { Prac_Variables.is_disable_Pause = s; });

        static private Prac_Hotkey hotkey_unlock_all = new Prac_Hotkey(KeyCode.F5,"F5", "Unlock all stage", (bool s) => { 
            hotkey_unlock_all.isActivated = false;
            Sincos.unlock_ex=true;
            Sincos.unlock_ex2=true;
            for(int i=0;i<4;i++){
                for(int j = 0; j<8; j++) {
                    Sincos.unlock_pr[i, j]=17;
                }
            }
        },new Color(0.7f,0.9f,1.0f));
        static private Prac_Hotkey hotkey_BossKill = new Prac_Hotkey(KeyCode.F6, "F6", "Kill all", (bool s) => {
            hotkey_BossKill.isActivated = false;
            foreach (var boss in Sincos.boss) {
                boss.life = -1;
            }
            foreach(var enm in Sincos.enemy)
            {
                var enmm = enm.gameObject.GetComponent<Enemymove>();
                if(enmm !=null){
                    enmm.life = -1;
                }
            }
        }, new Color(0.7f, 0.9f, 1.0f));

        static private Prac_Hotkey hotkey_Faster = new Prac_Hotkey(KeyCode.F7, "F7", "Faster15f", (bool s) => {
            hotkey_Faster.isActivated = false;
            // do not change Sincos.FPS since it will make game wrong
            Prac_Variables.gameFPS += 15;
            if (Prac_Variables.gameFPS>180)
                Prac_Variables.gameFPS=180;
        }, new Color(1.0f, 0.7f, 0.7f));

        static private Prac_Hotkey hotkey_Slower = new Prac_Hotkey(KeyCode.F8, "F8", "Slower15f", (bool s) => {
            hotkey_Slower.isActivated = false;

            Prac_Variables.gameFPS -= 15;
            if (Prac_Variables.gameFPS<60)
                Prac_Variables.gameFPS=60;
        }, new Color(1.0f, 0.7f, 0.7f));
        static private Prac_Hotkey hotkey_fasterBGM = 
            new Prac_Hotkey(KeyCode.F9, "F9", "FasterBGM", (bool s) => { Prac_Variables.is_fasterBGM = s; }, 
                new Color(1.0f, 0.7f, 0.7f));

        static public void OnGUI(){
            if (isOpen) {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.Label($"t1 = {Sincos.gametime}");
                GUILayout.Label($"t2 = {Sincos.gametime_beifen}");
                GUILayout.Label($"prac = {Prac_JmpGUIPatches.isPrac}");
                GUILayout.Label("  ");
                hotkey_invincable.OnGUI();
                hotkey_disable_X.OnGUI();
                hotkey_ctrl_spdup.OnGUI();
                hotkey_disable_pause.OnGUI();
                GUILayout.Label("  ");
                hotkey_unlock_all.OnGUI();
                hotkey_BossKill.OnGUI();
                GUILayout.Label("  ");
                hotkey_Faster.OnGUI();
                hotkey_Slower.OnGUI();
                hotkey_fasterBGM.OnGUI();
                GUILayout.Label($"FPS={Prac_Variables.gameFPS}");
                GUILayout.EndArea();
                // GUILayout.Window(114514, windRc, Window, "tools");
                if (UnityEngine.Input.GetKeyUp(KeyCode.Backspace) && wait_time>10){
                    isOpen = false;
                    wait_time = 0;
                }
            }else if (UnityEngine.Input.GetKeyUp(KeyCode.Backspace) && wait_time>10) {
                isOpen = true;
                wait_time = 0;
            }
            wait_time++;
        }
    }
    static class Prac_JmpGUIPatches
    {
        public struct StageFace
        {
            public string name;
            public int type;//0: stage, 1: midboss1, 2: midboss2
            public int time;
            public StageFace(string name, int time, int type)
            {
                this.name = name;
                this.type = type;
                this.time = time;
            }
        }
        public static List<StageFace>[] stageFace_2 = new List<StageFace>[]
        {
            // stage 1
            new List<StageFace>
            {
                new StageFace("front 1"                         ,  0    ,       0),
                new StageFace("front 2(after title)"            ,  2000 ,       0),//enemy 2
                new StageFace("front 3(big butterfly)"          ,  3400 ,       0),//enemy 7
                new StageFace("front 4(middle big butterfly)"   ,  5300 ,       0),//enemy 9
                new StageFace("mid normal 1"                    ,  6729 ,       1),
                new StageFace("mid spell 1"                     ,  6729 ,       2),
                new StageFace("later 1"                         ,  9200 ,       3),//enemy 0
                new StageFace("later 2(ju)"                     , 10500 ,       3),//enemy 2
                new StageFace("later 3(middle big butterfly)"   , 12150 ,       3),//enemy 6
            },// stage 2
            new List<StageFace>
            {
                new StageFace("front 1"                             ,  0    ,       0),
                new StageFace("front 2(after title)"                ,  3190 ,       0),//DHD 10
                new StageFace("front 3(red blue ju)"                ,  6890 ,       0),//PTYJ 30
                new StageFace("front 4(big butterfly)"              ,  8600 ,       0),//DHD 30
                new StageFace("mid spell 1"                         ,  9679 ,       1),
                new StageFace("later 1"                             ,  11000,       3),
            },
            // stage 3
            new List<StageFace>
            {
                new StageFace("front 1"                          ,  0    ,       0),
                new StageFace("front 2(after title)"             ,  3500 ,       0),//PTYJ  1-2
                new StageFace("front 3(big butterfly)"           ,  5200 ,       0),//DHD   1-3
                new StageFace("front 4(//\\\\)"                  ,  6800 ,       0),//PTYJ  3-2
                new StageFace("mid spell 1"                      ,  8249 ,       1),
                // new StageFace("mid spell 2"                      ,  8249 ,       2),
                new StageFace("later 1(ghost bottom)"            ,  12300 ,      3),//WGYJ 10
                new StageFace("later 2(ghost around)"            ,  15600 ,      3),//WGYJ 15
                new StageFace("later 3(snow butterfly)"          ,  17260 ,      3),//DHD 10
            },
            // stage 4
            new List<StageFace>
            {
                new StageFace("front 1"                         ,  0,          0),
                new StageFace("front 2(yaojing)"                ,  730,        0),
                new StageFace("front 3(circles)"                ,  1480,       0),
                new StageFace("front 4(after title)"            ,  4400,       0),
                new StageFace("front 5(2 wuke)"                 ,  5200,       0),
                new StageFace("front 6(<|-- ghost)"             ,  7200,       0),
                new StageFace("front 7(feixingzhen)"            ,  8190,       0),  //e17
                new StageFace("front 8(red ghost ju)"           ,  11380,       0), //e25
                new StageFace("front 9(black white circles)"    ,  13900,       0), //e30
                new StageFace("front 10(double zoufangdaren)"   ,  15000,       0), //e32
            }, // stage 5
            new List<StageFace>
            {
                new StageFace("front 1"                      ,  0    ,    0),
                new StageFace("front 2(red yaojing)"         ,  1500 ,    0),
                new StageFace("front 3(laser yaojing)"       ,  2900    , 0),
                new StageFace("front 4(horizonal laser)"     ,  4100    , 0),
                new StageFace("mid normal 1"                 ,  7199 ,    1),
                new StageFace("mid spell 1"                  ,  7199 ,    2),
                new StageFace("later 1(laser yaojing)"       ,  12500,    3),
                new StageFace("later 2(laser+bullet)"        ,  13400,    3),
                new StageFace("later 3(laser+laser)"         ,  16300,    3),
                new StageFace("later 4(blue mao)"            ,  19100,    3),
            }, // stage 6
            new List<StageFace>
            {
                new StageFace("front 1"                         ,     0,       0),
                new StageFace("front 2(blue)"                   ,  1500,       0),//e5
                new StageFace("front 3(blue ghosts)"            ,  2995,       0),//e6_1
                new StageFace("front 4(red ghosts+purple)"      ,  4950,       0),//e8_1
                new StageFace("front 5(blue mao)"               ,  8470,       0),//e12
                new StageFace("front 6(laser+ghosts2)"          ,  9900,       0),//e13
                new StageFace("front 7(ghosts final)"           , 13350,       0),//e17
            },
        };
        public struct BossFace
        {
            public string name;
            public int n_face;
            public int n_path;
            public bool is_spell;
            public int finalsp_stage;
            public BossFace(string name, int n_face,int n_path,bool is_spell,int finalsp_stage = 0)
            {
                this.name=name;
                this.n_face=n_face;
                this.n_path=n_path;
                this.is_spell=is_spell;
                this.finalsp_stage = finalsp_stage;
            }
        }

        public static List<BossFace>[] bossFaces_2 = new List<BossFace>[]
        {
            // stage 1
            new List<BossFace>
            { 
              new BossFace("normal 1",  0, 1,false),
              new BossFace("card 1",    1, 2,true),
              new BossFace("normal 2",  2, 3,false),
              new BossFace("card 2",    3, 4,true),
              new BossFace("card 3",    4, 5,true),
            },// stage 2
            new List<BossFace>
            {
              new BossFace("normal 1",  0, 1,false),
              new BossFace("card 1",    1, 2,true),
              new BossFace("normal 2",  2, 3,false),
              new BossFace("card 2",    3, 4,true),
              new BossFace("card 3",    4, 5,true),
            },
            // stage 3
            new List<BossFace>
            {
              new BossFace("normal 1",  0, 1,false),
              new BossFace("card 1",    1, 2,true),
              new BossFace("card 2",    2, 3,true),
              new BossFace("normal 2",  3, 4,false),
              new BossFace("card 3",    4, 5,true),
              new BossFace("normal 3",  5, 6,false),
              new BossFace("card 4",    6, 7,true),
            },
            // stage 4
            new List<BossFace>
            {
              new BossFace("normal 1",  0, 1,false),
              new BossFace("card 1",    1, 2,true),
              new BossFace("card 2",    2, 3,true),
              new BossFace("normal 2",  3, 4,false),
              new BossFace("card 3",    4, 5,true),
              new BossFace("card 4",    5, 6,true),
              new BossFace("normal 1+2",    -2, -2,false,-2),
              // new BossFace("card 5",    6, 7,true),
            }, // stage 5
            new List<BossFace>
            {
              new BossFace("normal 1",  0, 1,false),
              new BossFace("card 1",    1, 2,true),
              new BossFace("normal 2",  2, 3,false),
              new BossFace("card 2",    3, 4,true),
              new BossFace("normal 3",  4, 5,false),
              new BossFace("card 3",    5, 6,true),
              new BossFace("normal 4",  6, 7,false),
              new BossFace("card 4",    7, 8,true),
              //new BossFace("card 5",    8, 9,true),
            }, // stage 6
            new List<BossFace>
            {
              new BossFace("normal 1",  0, 1,false),
              new BossFace("card 1",    1, 2,true),
              new BossFace("normal 2",  2, 3,false),
              new BossFace("card 2",    3, 4,true),
              new BossFace("normal 3",  4, 5,false),
              new BossFace("card 3",    5, 6,true),
              new BossFace("normal 4",  6, 7,false),
              new BossFace("card 4",    7, 8,true),
              new BossFace("card 5",    8, 9,true),

              new BossFace("card 6 P1", 9, 10,true),
              new BossFace("card 6 P2", 9, 10,true, 1),
              new BossFace("card 6 P3", 9, 10,true, 2),
              new BossFace("card 6 P4", 9, 10,true, 3),

              new BossFace("normal 2+3", -1, -1,false, -1),
            },
        };
        public enum PracJmpSelectState
        {
            Open,Closed,OnEnter
        };
        public static PracJmpSelectState pracJmpSelectState = PracJmpSelectState.Closed;
        public static int pracJmpStage = 0;
        public static int pracJmpBoss = 0;
        public static int pracJmpStageFace = 0;
        public static int pracJmpBossFace = 0;
        public static Anniu_xuanzhong instance_selected;
        public static int time_wait = 0;
        public static bool isPrac = false;

        public static float res_life = 8;   //Sincos.jiemian_life, life = 1 when have 0 life
        public static float res_life_peice_a = 0; //Sincos.jiemian_lifepice_a
        public static float res_life_peice_b_index = 1; //Sincos.jiemian_lifepice_b, Sincos.lifepice_count[Sincos.lifepice_index]
        public static int res_life_peice_b = 20;
        public static float res_border_percent = 300.00f; // Sincos.jiejiefanghu_val, 0~99999 = 0, 100000~199999 = 1,200000~299998 = 2, 299999 = 3
        // 24*60*60*2, Sincos.daojishi
        public static float res_time_hour = 24;
        public static float res_time_min = 0;
        public static float res_time_sec = 0;

        public static int res_graze = 0; // Sincos.cadanshu
        public static int res_dedian = 0; // Sincos.jiemian_dedian
        public static int res_fenshu = 0; // Sincos.fenshu_now
        private static string graze_str="0";
        private static string dedian_str="0";
        private static string fenshu_str="0";

        public static bool res_has_LSC = true;

        public static Dictionary<int, List<int>> life_peice_b_diff // Sincos.select_nandu
            = new Dictionary<int, List<int>>()
            {
                {0,  new List<int>(){ 9, 9, 12, 12, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 } },
                {1,  new List<int>(){ 9, 9, 12, 12, 15, 15, 18, 18, 20, 20, 20, 20, 99, 99 } },
                {2,  new List<int>(){ 9, 12, 15, 18, 20, 25, 30, 30, 30, 30, 99, 99, 99, 99 } },
                {3,  new List<int>(){ 9, 12, 15, 20, 25, 30, 35, 40, 40, 99, 99, 99, 99, 99 } },
                {4,  new List<int>(){ 9, 12, 15, 20, 25, 30, 35, 40, 99, 99, 99, 99, 99, 99 } },
                {5,  new List<int>(){ 9, 12, 15, 20, 25, 30, 35, 40, 99, 99, 99, 99, 99, 99 } },
            };

        static private Rect windRc = new Rect(100, 100, 200, 200);
        public static void OnGUI()
        {
            if(pracJmpSelectState == PracJmpSelectState.Open) {
                Cursor.visible = true;
                windRc.center = new Vector2(Screen.width/2,Screen.height/2);
                windRc.size = new Vector2(Screen.width*0.4f,Screen.height*0.4f);
                GUILayout.Window(1919810, windRc, Window, "select");
            }
        }
        static private void Window(int id)
        {
            
            time_wait++;
            int time_wait_max = 10;

            // selects
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("    ");
            GUILayout.Label("---JUMP---");
            if (pracJmpStage <= 0){
                var cur_Faces = bossFaces_2[pracJmpBoss - 1];
                if (time_wait>=time_wait_max && (UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Right) || UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Down)))
                {
                    time_wait = 0;
                    pracJmpBossFace++;
                    if (pracJmpBossFace >= cur_Faces.Count)
                        pracJmpBossFace = 0;
                }
                if (time_wait>=time_wait_max && (UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Left) || UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Up)))
                {
                    time_wait = 0;
                    pracJmpBossFace--;
                    if (pracJmpBossFace < 0)
                        pracJmpBossFace = cur_Faces.Count - 1;
                }
                for(int i=0;i<cur_Faces.Count;i++)
                {
                    GUIStyle activated_style = new GUIStyle(GUI.skin.label);
                    if (i == pracJmpBossFace)
                    {
                        activated_style.normal.textColor = Color.green;
                        activated_style.fontStyle = FontStyle.Bold;
                    }else{
                        activated_style.normal.textColor = Color.white;
                    }
                    GUILayout.Label(cur_Faces[i].name, activated_style);
                }
            }
            else
            {
                var cur_stages = stageFace_2[pracJmpStage - 1];
                if (time_wait>=time_wait_max && (UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Right) || UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Down)))
                {
                    time_wait = 0;
                    pracJmpStageFace++;
                    if (pracJmpStageFace >= cur_stages.Count)
                        pracJmpStageFace = 0;
                }
                if (time_wait>=time_wait_max &&  (UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Left) || UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_Up)))
                {
                    time_wait = 0;
                    pracJmpStageFace--;
                    if (pracJmpStageFace < 0)
                        pracJmpStageFace = cur_stages.Count - 1;
                }
                for (int i = 0; i<cur_stages.Count; i++)
                {
                    GUIStyle activated_style = new GUIStyle(GUI.skin.label);
                    if (i == pracJmpStageFace)
                    {
                        activated_style.normal.textColor = Color.green;
                        activated_style.fontStyle = FontStyle.Bold;
                    } else {
                        activated_style.normal.textColor = Color.white;
                    }
                    GUILayout.Label(Prac_Locale.GetLocaleString(cur_stages[i].name), activated_style);
                }
            }
            GUILayout.Label("---------");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("    ");
            GUILayout.Label("---RES---");

            {
                GUILayout.BeginHorizontal();
                res_life = GUILayout.HorizontalSlider(res_life, 0.0f, 8.0f);
                res_life = (float)Math.Round(res_life);
                GUILayout.Label($"{Prac_Locale.GetLocaleString("life")}: {res_life}");
                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();
                res_life_peice_a = GUILayout.HorizontalSlider(res_life_peice_a, 0.0f, res_life_peice_b==99?99: res_life_peice_b-1);
                res_life_peice_b_index = GUILayout.HorizontalSlider(res_life_peice_b_index, 0.0f, 13.0f);
                var life_peice_bs = life_peice_b_diff[Sincos.select_nandu];

                res_life_peice_a = (float)Math.Round(res_life_peice_a);
                res_life_peice_b_index = (float)Math.Round(res_life_peice_b_index);

                if (res_life_peice_b_index < 0.0f)
                    res_life_peice_b_index = 0;
                if(res_life_peice_b_index > life_peice_bs.Count)
                    res_life_peice_b_index = life_peice_bs.Count;

                if (res_life_peice_a < 0.0f)
                    res_life_peice_a = 0;

                res_life_peice_b = life_peice_bs[(int)res_life_peice_b_index];
                if (res_life_peice_a >= res_life_peice_b)
                    res_life_peice_a = res_life_peice_b==99 ? 99 : res_life_peice_b-1;

                GUILayout.Label($"{Prac_Locale.GetLocaleString("lifepiece")}: {res_life_peice_a}/{life_peice_bs[(int)res_life_peice_b_index]}");
                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();
                res_border_percent = GUILayout.HorizontalSlider(res_border_percent, 0.0f, 300.000f);
                int res_border_int = (int)(res_border_percent * 100000.0f/100.0f);
                if(res_border_int < 299999)
                    GUILayout.Label($"{Prac_Locale.GetLocaleString("border")}: {Math.Floor(res_border_percent)} %");
                else
                    GUILayout.Label($"{Prac_Locale.GetLocaleString("border")}: 300 %");
                GUILayout.EndHorizontal();
                GUILayout.Label($"{Prac_Locale.GetLocaleString("border_desc")}");
            }
            {
                GUILayout.BeginHorizontal();
                res_time_hour = GUILayout.HorizontalSlider(res_time_hour, 0.0f, 24f);
                res_time_min = GUILayout.HorizontalSlider(res_time_min, 0.0f, 59f);
                res_time_sec = GUILayout.HorizontalSlider(res_time_sec, 0.0f, 59f);

                res_time_hour = (float)Math.Round(res_time_hour);
                res_time_min = (float)Math.Round(res_time_min);
                res_time_sec = (float)Math.Round(res_time_sec);
                if (res_time_hour == 24)
                {
                    res_time_min = 0;
                    res_time_sec = 0;
                }
                GUILayout.Label($"{Prac_Locale.GetLocaleString("time")}: {res_time_hour}:{res_time_min}:{res_time_sec}");
                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();
                graze_str = GUILayout.TextArea(graze_str);
                var succ = int.TryParse(graze_str,out res_graze);
                if(!succ){
                    graze_str = "0";
                    res_graze = 0;
                }
                GUILayout.Label($"{Prac_Locale.GetLocaleString("graze")}: {res_graze}");
                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();
                dedian_str = GUILayout.TextArea(dedian_str);
                var succ = int.TryParse(dedian_str, out res_dedian);
                if (!succ)
                {
                    dedian_str = "0";
                    res_dedian = 0;
                }
                GUILayout.Label($"{Prac_Locale.GetLocaleString("dian")}: {res_dedian*10}");
                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();
                fenshu_str = GUILayout.TextArea(fenshu_str);
                var succ = int.TryParse(fenshu_str, out res_fenshu);
                if (!succ)
                {
                    fenshu_str = "0";
                    res_fenshu = 0;
                }
                if(res_fenshu==0)
                    GUILayout.Label($"{Prac_Locale.GetLocaleString("score")}: 0");
                else
                    GUILayout.Label($"{Prac_Locale.GetLocaleString("score")}: {res_fenshu}0");
                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();
                string str = $"{Prac_Locale.GetLocaleString("LSC")}: {res_has_LSC}";
                if (GUILayout.Button(str))
                    res_has_LSC = !res_has_LSC;
               GUILayout.EndHorizontal();
            }
            
            GUILayout.Label("---------");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
           


            if (time_wait>=time_wait_max){
                bool is_key_down = false;
                foreach(var key in m_StandaloneInputModule.Instance.m_SubmitKeys)
                {
                    if(UnityEngine.Input.GetKeyDown(key))
                    {
                        is_key_down=true;
                        break;
                    }
                }
                if(is_key_down) {
                    time_wait = 0;
                    pracJmpSelectState = PracJmpSelectState.OnEnter;
                    instance_selected.Pr_select(pracJmpStage, pracJmpBoss);
                    isPrac = true;
                }
            }
            if (time_wait>=time_wait_max)
            {
                bool is_key_down = false;
                if (UnityEngine.Input.GetKeyDown(m_StandaloneInputModule.Instance.m_ESC))
                    is_key_down=true;
                if(!is_key_down)
                    foreach (var key in m_StandaloneInputModule.Instance.m_CancelKeys){
                        if (UnityEngine.Input.GetKeyDown(key))
                        {
                            is_key_down=true;
                            break;
                        }
                    }
                if (is_key_down) {
                    isPrac = false;
                    time_wait = 0;
                    Cursor.visible = false;
                    pracJmpSelectState = PracJmpSelectState.Closed;
                    foreach (var prs in instance_selected.pr_stage){
                        prs.interactable = true;
                    }
                    foreach (var prs in instance_selected.pr_boss){
                        prs.interactable = true;
                    }
                    if (pracJmpStage>0) {
                        instance_selected.eventsys.GetComponent<EventSystem>().SetSelectedGameObject(instance_selected.pr_stage[pracJmpStage-1].gameObject);
                    }else{
                        instance_selected.eventsys.GetComponent<EventSystem>().SetSelectedGameObject(instance_selected.pr_boss[pracJmpBoss-1].gameObject);
                    }
                }
                
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Anniu_xuanzhong), "OnButtonSubmit")]
        public static void Anniu_xuanzhong_OnButtonSubmit_PracJmpPatch(Anniu_xuanzhong __instance, GameObject go)
        {
            if(go == __instance.gamestart.gameObject || go == __instance.exstart.gameObject || go == __instance.spellpractice.gameObject)
            {
                isPrac = false;
            }
            if (pracJmpSelectState == PracJmpSelectState.Closed) {
                bool has_null = false;
                foreach (var prs in __instance.pr_stage){
                    if(prs!=null) prs.interactable = true;
                    else has_null= true;
                }
                foreach (var prs in __instance.pr_boss)
                {
                    if (prs!=null) prs.interactable = true;
                    else has_null= true;
                }
                if (!has_null)
                {
                    __instance.pr_stage[0].navigation =  new Navigation
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnRight = __instance.pr_boss[0],
                        selectOnLeft = __instance.pr_boss[0],
                        selectOnUp = __instance.pr_stage[5],
                        selectOnDown = __instance.pr_stage[1],
                    };
                    __instance.pr_boss[0].navigation = new Navigation
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnRight = __instance.pr_stage[0],
                        selectOnLeft = __instance.pr_stage[0],
                        selectOnUp = __instance.pr_boss[5],
                        selectOnDown = __instance.pr_boss[1],
                    };
                    for (int i=1;i<=4;i++)
                    {
                        __instance.pr_stage[i].navigation = new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnRight = __instance.pr_boss[i],
                            selectOnLeft = __instance.pr_boss[i],
                            selectOnDown = __instance.pr_stage[i+1],
                            selectOnUp = __instance.pr_stage[i-1],
                        };
                        __instance.pr_boss[i].navigation =  new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnRight = __instance.pr_stage[i],
                            selectOnLeft = __instance.pr_stage[i],
                            selectOnDown = __instance.pr_boss[i+1],
                            selectOnUp = __instance.pr_boss[i-1],
                        };
                    }
                    __instance.pr_stage[5].navigation =  new Navigation
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnRight = __instance.pr_boss[5],
                        selectOnLeft = __instance.pr_boss[5],
                        selectOnUp = __instance.pr_stage[4],
                        selectOnDown = __instance.pr_stage[0],
                    };
                    __instance.pr_boss[5].navigation = new Navigation
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnRight = __instance.pr_stage[5],
                        selectOnLeft = __instance.pr_stage[5],
                        selectOnUp = __instance.pr_boss[4],
                        selectOnDown = __instance.pr_boss[0],
                    };
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Anniu_xuanzhong), "Pr_select")]
        public static bool Anniu_xuanzhong_Pr_select_PracJmpPatch(Anniu_xuanzhong __instance, int stage, int boss)
        {
            pracJmpStage = stage;
            pracJmpBoss = boss;
            if (pracJmpSelectState == PracJmpSelectState.OnEnter){
                Cursor.visible = false;
                pracJmpSelectState = PracJmpSelectState.Closed;
                return true;
            }
            pracJmpSelectState = PracJmpSelectState.Open;
            pracJmpBossFace = 0; 
            pracJmpStageFace = 0;
            instance_selected = __instance;
            foreach (var prs in __instance.pr_stage){
                prs.interactable = false;
            }
            foreach (var prs in __instance.pr_boss){
                prs.interactable = false;
            }
            time_wait = 0;
            return false;
        }

    }
    class Prac_JmpPatches
    {

        [HarmonyPostfix, HarmonyPatch(typeof(Loading0), "Awake")]
        public static void Loading0_Awake_PracJmpPatch(Loading0 __instance)
        {
            if (Prac_JmpGUIPatches.isPrac == false)
            {
                if(!Sincos.ifreplay)
                {
                    // isPrac == false, replay == false
                    return;
                }
                else
                {
                    //isPrac == false, replay == true
                    if (Sincos.replay_nandu >= 6 && Sincos.replay_nandu <= 9)
                    {
                        //isPrac == false, replay == true, practice mode == true
                        if (__instance.gameObject.GetComponent<Jiejie_add>() != null)
                            UnityEngine.Object.Destroy(__instance.gameObject.GetComponent<Jiejie_add>());
                        Sincos.jiemian_life = Sincos.replay_life[Sincos.replay_start_mian];
                        return;
                    }
                    else
                    {
                        //isPrac == false, replay == true, practice mode == false
                        return;
                    }
                }
            }

            // isPrac == true
            if (Sincos.ifreplay)
            {
                // isPrac == true, replay == true
                if (Sincos.replay_nandu >= 6 && Sincos.replay_nandu <= 9)
                {
                    //isPrac == true, replay == true, practice mode == true
                    if (__instance.gameObject.GetComponent<Jiejie_add>() != null)
                        UnityEngine.Object.Destroy(__instance.gameObject.GetComponent<Jiejie_add>());
                    Sincos.jiemian_life = Sincos.replay_life[Sincos.replay_start_mian];
                    return;
                }
                else
                {
                    //isPrac == false, replay == true, practice mode == false
                    return;
                }
            }

            // isPrac == true, replay == false
            if (__instance.gameObject.GetComponent<Jiejie_add>()!=null)
                UnityEngine.Object.Destroy(__instance.gameObject.GetComponent<Jiejie_add>());

            Sincos.jiemian_life = (int)Math.Round(Prac_JmpGUIPatches.res_life) + 1;
            Sincos.jiemian_lifepice_a = (int)Math.Round(Prac_JmpGUIPatches.res_life_peice_a);
            Sincos.lifepice_index = (int)Math.Round(Prac_JmpGUIPatches.res_life_peice_b_index);
            Sincos.jiemian_lifepice_b = Prac_JmpGUIPatches.res_life_peice_b;

            int res_border_int = (int)(Prac_JmpGUIPatches.res_border_percent * 100000.0f/100.0f);
            if (res_border_int < 299999)
                Sincos.jiejiefanghu_val = res_border_int;
            else
                Sincos.jiejiefanghu_val = 299999;

            if(res_border_int < 100000)
                Sincos.jiejie_cengshu = 0;
            else if(res_border_int < 200000)
                Sincos.jiejie_cengshu = 1;
            else if(res_border_int < 299999)
                Sincos.jiejie_cengshu = 2;
            else
                Sincos.jiejie_cengshu = 3;

            Sincos.fenshu_now = Prac_JmpGUIPatches.res_fenshu;
            Sincos.daojishi =(((int)Math.Round(Prac_JmpGUIPatches.res_time_hour))*3600
                +((int)Math.Round(Prac_JmpGUIPatches.res_time_min))*60
                +((int)Math.Round(Prac_JmpGUIPatches.res_time_sec)))*2;
            Sincos.cadanshu = Prac_JmpGUIPatches.res_graze;
            Sincos.jiemian_dedian = Prac_JmpGUIPatches.res_dedian*10;

            Sincos.replay_start_mian -=1;
            Sincos.replay_LSC = Prac_JmpGUIPatches.res_has_LSC ? 1:0;
            Sincos.replay_life[Sincos.replay_start_mian] = Sincos.jiemian_life;
            Sincos.replay_life_piece[Sincos.replay_start_mian] = Sincos.jiemian_lifepice_a;
            Sincos.replay_life_index[Sincos.replay_start_mian] = Sincos.lifepice_index;
            Sincos.replay_jiejie_val[Sincos.replay_start_mian] = Sincos.jiejiefanghu_val;
            Sincos.replay_jiejie_cengshu[Sincos.replay_start_mian] = Sincos.jiejie_cengshu;
            Sincos.replay_fenshu[Sincos.replay_start_mian] = Sincos.fenshu_now;
            Sincos.replay_time_daojishi[Sincos.replay_start_mian] = Sincos.daojishi;
            Sincos.replay_dedian[Sincos.replay_start_mian] = Sincos.jiemian_dedian;
            Sincos.replay_graze[Sincos.replay_start_mian] = Sincos.cadanshu;
            Sincos.replay_start_mian +=1;
            // Sincos.replay_jiejie_jianglicishu[Sincos.replay_start_mian] = Sincos.jiejie_jianglicishu;
        }


        [HarmonyPrefix, HarmonyPatch(typeof(Bossmove), "FixedUpdate")]
        public static void Bossmove_FixedUpdate_PracJmpPatch(Bossmove __instance)
        {
            if (Prac_JmpGUIPatches.isPrac==false)
                return;
            if (Prac_JmpGUIPatches.pracJmpStage <= 0 && Prac_JmpGUIPatches.pracJmpBossFace > 0)
            {
                var face = Prac_JmpGUIPatches.bossFaces_2[Prac_JmpGUIPatches.pracJmpBoss - 1][Prac_JmpGUIPatches.pracJmpBossFace];
                if (__instance.fuka == 0)
                {
                    if(__instance.double_boss && __instance.double_boss2)
                        __instance.fuka = face.n_face;
                    else
                        __instance.fuka = face.n_face - 1;
                    if (face.is_spell)
                    {
                        // __instance.life = -1;
                        // init : 9999999
                        if (Sincos.danm_daojishi < 500000 && Sincos.danm_daojishi > 30)
                        {
                            Sincos.danm_daojishi = 10;
                        }
                    }
                }
                else if (__instance.fuka == 9 && face.n_face == 9 && Prac_JmpGUIPatches.pracJmpBoss == 6 && face.finalsp_stage != 0)
                {
                    //stage 6 final
                    if (__instance.life < 300000 &&  __instance.life >= 0)
                    {
                        if (face.finalsp_stage == 1 &&  __instance.life > 42500)//P2
                        {
                            __instance.life = 42500;
                        }
                        else if (face.finalsp_stage == 2 &&  __instance.life > 30000)//P3
                        {
                            __instance.life = 30000;
                        }
                        else if (face.finalsp_stage == 3 &&  __instance.life > 15000)//P4
                        {
                            __instance.life = 15000;
                        }
                    }
                }
            }
            else if (Prac_JmpGUIPatches.pracJmpStage > 0 && Prac_JmpGUIPatches.pracJmpStageFace > 0)
            {
                var face = Prac_JmpGUIPatches.stageFace_2[Prac_JmpGUIPatches.pracJmpStage - 1][Prac_JmpGUIPatches.pracJmpStageFace];
                if (face.type==2 && __instance.fuka == 0)
                { // mid 2
                    if (Sincos.danm_daojishi < 500000 && Sincos.danm_daojishi > 30)
                    {
                        Sincos.danm_daojishi = 10;
                    }
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Boss_in), "Zairudanmu")]
        public static void Boss_in_Zairudanmu_PracJmpPatch(Boss_in __instance)
        {
            if (Prac_JmpGUIPatches.isPrac==false)
                return;
            Debug.Log($"Boss_in_Zairudanmu_PracJmpPatch === {__instance.path}, {__instance.bm.fuka},{Prac_JmpGUIPatches.pracJmpStage}");
            if (Prac_JmpGUIPatches.pracJmpStage <= 0)
            {
                var face = Prac_JmpGUIPatches.bossFaces_2[Prac_JmpGUIPatches.pracJmpBoss - 1][Prac_JmpGUIPatches.pracJmpBossFace];
                if (!face.is_spell)
                {
                    if(face.n_face==-1)// 6boss normal 2+3 special
                    {
                        __instance.bm.fuka = 3;
                        int idx = __instance.path.LastIndexOf('/');
                        __instance.path = __instance.path.Substring(0, idx) + "/" + "3";
                        __instance.aa = __instance.path;
                       
                    }else if(face.n_face==-2){// 4boss normal 1+2
                        __instance.bm.fuka = 2;
                        int idx = __instance.path.LastIndexOf('/');
                        __instance.path = __instance.path.Substring(0, idx) + "/" + "1";
                        __instance.aa = __instance.path;
                    }
                    else
                    {
                        __instance.bm.fuka = face.n_face;
                        int idx = __instance.path.LastIndexOf('/');
                        __instance.path = __instance.path.Substring(0, idx) + "/" + $"{face.n_path}";
                        __instance.aa = __instance.path;
                    }
                }
                else
                {
                    __instance.bm.fuka = 0;
                }
                Debug.Log($"face={face.name},{face.n_path},{face.is_spell}");
            }
            else if (Prac_JmpGUIPatches.pracJmpStage > 0 && Prac_JmpGUIPatches.pracJmpStageFace > 0)
            {
                var face = Prac_JmpGUIPatches.stageFace_2[Prac_JmpGUIPatches.pracJmpStage - 1][Prac_JmpGUIPatches.pracJmpStageFace];
                if (face.type==2)
                { // mid 2
                    if (Prac_JmpGUIPatches.pracJmpStage==3)
                    {// stage 3 spell 2
                        // todo...
                    }
                    else
                    {
                        __instance.bm.fuka = 0;
                    }
                }
            }
            Debug.Log($"Boss_in_Zairudanmu_PracJmpPatch ==== {__instance.path}");
        }

        public static void Zairudanmu_path(string Dpath, string b_path)
        {
            var dam = (Resources.Load("xuelianhua/prefab/dam") as GameObject);
            if (Sincos.iftest){
                FileInfo[] files;
                DirectoryInfo directoryInfo = new DirectoryInfo(Dpath + "/" + b_path);
                files = directoryInfo.GetFiles();
                foreach (Basic basic in Sincos.fasheqi){
                    if (basic != null)
                        UnityEngine.Object.Destroy(basic.gameObject);
                }
                List<FileInfo> list = (from a in files
                                       where Path.GetExtension(a.Name) == ".txt"
                                       select a).ToList<FileInfo>();
                Sincos.fasheqi = new Basic[list.Count];
                for (int j = 0; j < list.Count; j++)
                {
                    FileInfo fileInfo = list[j];
                    Sincos.fasheqi[j] = UnityEngine.Object.Instantiate<GameObject>(dam, new Vector3(0f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<Basic>();
                    Sincos.fasheqi[j].path = fileInfo.FullName;
                    Sincos.fasheqi[j].name0 = fileInfo.Name;
                    Sincos.fasheqi[j].if_fenjieduan = false;
                    Sincos.fasheqi[j].if_dontresetbossmove = false;
                }
                return;
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Boss_creat), "FixedUpdate")]
        public static void Boss_creat_FixedUpdate_PracJmpPatch(Boss_creat __instance)
        {
            if (Prac_JmpGUIPatches.isPrac==false)
                return;
            if (Sincos.gametime > __instance.time_in + 1)
            {
                Bossmove boss=null;
                switch (Sincos.select_nandu)
                {
                    case 0:
                        boss = __instance.boss_E.GetComponent<Bossmove>();
                        break;
                    case 1:
                        boss = __instance.boss_N.GetComponent<Bossmove>();
                        break;
                    case 2:
                        boss = __instance.boss_H.GetComponent<Bossmove>();
                        break;
                    case 3:
                        boss = __instance.boss_L.GetComponent<Bossmove>();
                        break;
                    case 4:
                        boss = __instance.boss_EXA.GetComponent<Bossmove>();
                        break;
                    case 5:
                        boss = __instance.boss_EXB.GetComponent<Bossmove>();
                        break;
                }
                if (boss!=null && boss.b_path!=null)
                {
                    Debug.Log($"Boss_creat_FixedUpdate_PracJmpPatch === {boss.b_path}");
                    Action ac7 = delegate ()
                    {
                        Zairudanmu_path(Sincos.GetAssetsPath(), boss.b_path);
                    };
                    Sincos.Renwu item9;
                    item9.ac = ac7;
                    item9.time = 1;
                    Sincos.renwu.Add(item9);
                }
            }
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(Timegame), "FixedUpdate")]
        public static void Timegame_FixedUpdate_PracJmpPatch(Timegame __instance)
        {
            if (Prac_JmpGUIPatches.isPrac==false)
                return;
            if (Prac_JmpGUIPatches.pracJmpStage > 0 && Prac_JmpGUIPatches.pracJmpStageFace > 0) {
                if(Sincos.gametime == 30 && Sincos.gametime == Sincos.gametime_beifen)
                {
                    var face = Prac_JmpGUIPatches.stageFace_2[Prac_JmpGUIPatches.pracJmpStage - 1][Prac_JmpGUIPatches.pracJmpStageFace];
                    Sincos.gametime = face.time;
                    Sincos.gametime_beifen = face.time - 1;// to avoid equal test
                    if (Sincos.BGM){
                        Sincos.BGM.time = face.time / 120.0f - 0.266f;
                    }
                    if (face.type==3){
                        var obj = UnityEngine.Object.FindObjectOfType(typeof(Boss_creat));
                        if (Sincos.boss.Count>=1){
                            Sincos.boss[0].Zairudanmu_path(Sincos.boss[0].b_path, false, "", false);
                        }
                    }
                }
            }
        }
    }
    class Prac_OverlayPatches
    {
        static SpriteRenderer[] miss_cnt = new SpriteRenderer[8];
        static SpriteRenderer[] break_cnt = new SpriteRenderer[8];
        static List<KeyCode> orig_Cancelkeys;

        [HarmonyPrefix, HarmonyPatch(typeof(Anniu_Set), "FixedUpdate")]
        public static void UnityEngine_GetKey_Patch1(Anniu_Set __instance,List<KeyCode> __state)
        {
            if(Prac_Variables.is_disable_X)
            {
                orig_Cancelkeys = new List<KeyCode>(m_StandaloneInputModule.Instance.m_CancelKeys);
                for(int i=0;i< m_StandaloneInputModule.Instance.m_CancelKeys.Count;i++){
                    m_StandaloneInputModule.Instance.m_CancelKeys[i] = KeyCode.None;
                }
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Anniu_Set), "FixedUpdate")]
        public static void UnityEngine_GetKey_Patch2(Anniu_Set __instance, List<KeyCode> __state)
        {
            if (Prac_Variables.is_disable_X){
                for (int i = 0; i< m_StandaloneInputModule.Instance.m_CancelKeys.Count; i++)
                {
                    m_StandaloneInputModule.Instance.m_CancelKeys[i] = orig_Cancelkeys[i];
                }
            }
            // replay pitch
            {
                if (Sincos.kuaijin_yunxu && Sincos.gametime >= 120 && Time.timeScale != 0f && Sincos.ifreplay==true)
                {
                    if ((UnityEngine.Input.GetKey(m_StandaloneInputModule.Instance.m_ctrl) || UnityEngine.Input.GetKey(m_StandaloneInputModule.Instance.mj_ctrl)))
                    {
                        Sincos.BGM.pitch = 3f;
                    }else if ((UnityEngine.Input.GetKey(m_StandaloneInputModule.Instance.m_slow) || UnityEngine.Input.GetKey(m_StandaloneInputModule.Instance.mj_slow)))
                    {
                        Sincos.BGM.pitch = 0.5f;
                    }
                    else
                    {
                        Sincos.BGM.pitch = 1f;
                    }
                }
                else
                {
                    if (Sincos.ifreplay==true && Sincos.BGM.pitch != 1f)
                        Sincos.BGM.pitch = 1f;
                }
                
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Jiemian_lifepice), "Start")]
        public static void Jiemian_lifepice_Start_UIPatch(Jiemian_lifepice __instance)
        {
            Debug.Log($"Jiemian_lifepice_Start_UIPatch ==== ");
            for (int i = 0; i<8; i++)
            {
                var gameobj = UnityEngine.Object.Instantiate(__instance.lifepice_a[0].gameObject, __instance.transform);
                var gameobj2 = UnityEngine.Object.Instantiate(__instance.lifepice_a[0].gameObject, __instance.transform);
                gameobj.name = $"miss_cnt{i}";
                gameobj2.name = $"break_cnt{i}";
                if (gameobj != null)
                {
                    Vector3 diff = __instance.lifepice_a[0].gameObject.transform.position - __instance.lifepice_a[1].gameObject.transform.position;
                    miss_cnt[i] = gameobj.GetComponent<SpriteRenderer>();
                    miss_cnt[i].transform.SetParent(__instance.transform);
                    miss_cnt[i].transform.position = __instance.lifepice_b[1].gameObject.transform.position + new Vector3(-diff.x*i, -diff.x*1.8f, 0);

                }
                if(gameobj2 != null)
                {
                    Vector3 diff = __instance.lifepice_a[0].gameObject.transform.position - __instance.lifepice_a[1].gameObject.transform.position;
                    break_cnt[i] = gameobj2.GetComponent<SpriteRenderer>();
                    break_cnt[i].transform.SetParent(__instance.transform);
                    break_cnt[i].transform.position = __instance.lifepice_b[1].gameObject.transform.position + new Vector3(-diff.x*i, -diff.x*3.6f, 0);
                }
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Jiemian_lifepice), "FixedUpdate")]
        public static void Jiemian_lifepice_FixedUpdate_UIPatch(Jiemian_lifepice __instance)
        {
            int num2 = Sincos.replay_misszongshu;
            for (int i = 0; i < 8; i++)
            {
                int digit = num2 % 10;
                if(num2==0 && i!=0){
                    miss_cnt[i].color = new Color(1f, 1f, 1f, 0f);
                }else{
                    miss_cnt[i].color = new Color(1f, 0.75f, 0.75f, 1f);
                }
                num2 /= 10;
                if (miss_cnt[i] != null){
                    miss_cnt[i].sprite = Sincos.font_lifepice[digit];
                }
            }
            int num3 = Sincos.replay_breakzongshu + Sincos.replay_shifangzongshu;
            for (int i = 0; i < 8; i++)
            {
                int digit = num3 % 10;
                if (num3==0 && i!=0)
                {
                    break_cnt[i].color = new Color(1f, 1f, 1f, 0f);
                }else{
                    break_cnt[i].color = new Color(0.75f, 0.75f, 1f, 1f);
                }
                num3 /= 10;
                if (break_cnt[i] != null)
                {
                    break_cnt[i].sprite = Sincos.font_lifepice[digit];
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Centeryidong), "FixedUpdate")]
        public static void Centeryidong_FixedUpdate_InvincablePatch(Centeryidong __instance)
        {
            if (Prac_Variables.is_invincable){
                if(Sincos.lift == false)
                {
                    Sincos.lift = true;
                    if(__instance.sound){
                        __instance.se.Play();
                    }
                    Sincos.bonuns = false;
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Zanting), "OnApplicationFocus")]
        public static bool Zanting_OnApplicationFocus_DisablePause(Zanting __instance)
        {
            if (Prac_Variables.is_disable_Pause)
            {
                return false;
            }
            return true;
        }
    }

    [BepInPlugin("xxx.RUE.ASL_Prac","ASL_Prac","0.0.7")]
    public class ASL_Prac_Mod:BaseUnityPlugin
    {
        void Start()
        {
            Logger.LogInfo("log");
            Harmony.CreateAndPatchAll(typeof(Prac_OverlayPatches));
            Harmony.CreateAndPatchAll(typeof(Prac_JmpGUIPatches));
            Harmony.CreateAndPatchAll(typeof(Prac_JmpPatches));
        }
        void Update()
        {
        }

        static float fixedDeltaT_saved = -1.0f;

        static Collection<string> sceneNames = new Collection<string> { 
            "Stage1" ,
            "Stage2" ,
            "Stage3" ,
            "Stage4" ,
            "Stage5" ,
            "Stage6" ,
            "Stage1_boss" ,
            "Stage2_boss" ,
            "Stage3_boss" ,
            "Stage4_boss" ,
            "Stage5_boss" ,
            "Stage6_boss" ,
            "Stage1_spell" ,
            "Stage2_spell" ,
            "Stage3_spell" ,
            "Stage4_spell" ,
            "Stage5_spell" ,
            "Stage6_spell" ,
            "StageEXA" ,
            "StageEXA_spell" ,
            "StageEXB" ,
        };
        void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.skin.button.fontSize = 32;
            GUI.skin.window.fontSize = 32;
            GUI.skin.horizontalSlider.fontSize = 32;
            GUI.skin.horizontalSlider.fixedHeight = 32;
            GUI.skin.horizontalSliderThumb.fixedHeight = 32;
            GUI.skin.horizontalSliderThumb.fixedWidth = 32;
            GUI.skin.textArea.fontSize = 32;
            GUI.skin.toggle.fontSize = 32;
            GUI.skin.box.fixedHeight = 32;
            GUI.skin.button.fixedHeight = 32;

            Prac_JmpGUIPatches.OnGUI();
            Prac_Overlay.OnGUI();
            if(fixedDeltaT_saved == -1.0f)
            {
                fixedDeltaT_saved = UnityEngine.Time.fixedDeltaTime;
            }
            
            if(sceneNames.Contains(SceneManager.GetActiveScene().name))
            {
                if (UnityEngine.Time.fixedDeltaTime != 1.0f/(Prac_Variables.gameFPS * 2.0f))
                {
                    Application.targetFrameRate = Convert.ToInt32(Sincos.FPS*(Prac_Variables.gameFPS/60.0f));
                    UnityEngine.Time.fixedDeltaTime = 1.0f/(Prac_Variables.gameFPS * 2.0f);
                    fixedDeltaT_saved =  1.0f/(Prac_Variables.gameFPS*2.0f);
                    if (Prac_Variables.is_fasterBGM)
                    {
                        Sincos.BGM.pitch = Prac_Variables.gameFPS/60.0f;
                    }
                    else
                    {
                        Sincos.BGM.pitch = 1.0f;
                    }
                }
            } else {
                if (UnityEngine.Time.fixedDeltaTime != 1.0f/(60.0f * 2.0f))
                {
                    Application.targetFrameRate = Sincos.FPS;
                    UnityEngine.Time.fixedDeltaTime = 1.0f/(60.0f * 2.0f);
                    fixedDeltaT_saved =  1.0f/(60.0f*2.0f);
                    if (Prac_Variables.is_fasterBGM)
                    {
                        Sincos.BGM.pitch = Prac_Variables.gameFPS/60.0f;
                    }
                    else
                    {
                        Sincos.BGM.pitch = 1.0f;
                    }
                }
            }


            if (Prac_Variables.is_ctrl_spdup)
            {
                if (UnityEngine.Input.GetKey(KeyCode.LeftControl)&& Sincos.replay_nandu >= 6 && Sincos.ifreplay == false) //in prac mode,Sincos.replay_nandu = nandu+6
                {
                    if (UnityEngine.Time.fixedDeltaTime != 1.0f/(120.0f*5.0f))
                    {
                        UnityEngine.Time.fixedDeltaTime = 1.0f/(120.0f*5.0f);
                        Sincos.BGM.pitch = 5.0f;
                    }
                }
                else
                {
                    if (UnityEngine.Time.fixedDeltaTime != fixedDeltaT_saved)
                    {
                        UnityEngine.Time.fixedDeltaTime = fixedDeltaT_saved;
                        if (Prac_Variables.is_fasterBGM)
                        {
                            Sincos.BGM.pitch = 120.0f/fixedDeltaT_saved;
                        }
                        else
                        {
                            Sincos.BGM.pitch = 1.0f;
                        }
                    }
                }

            }
            else
            {
                if (fixedDeltaT_saved != -1.0f && fixedDeltaT_saved != UnityEngine.Time.fixedDeltaTime)
                {
                    UnityEngine.Time.fixedDeltaTime = fixedDeltaT_saved;
                    if (Prac_Variables.is_fasterBGM)
                    {
                        Sincos.BGM.pitch = 120.0f/fixedDeltaT_saved;
                    }
                    else
                    {
                        Sincos.BGM.pitch = 1.0f;
                    }
                }
            }
        }
        
    }
}
