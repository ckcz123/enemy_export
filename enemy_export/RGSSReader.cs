using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace enemy_export
{
    class RGSSReader
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RGSSEval(IntPtr pScripts);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RGSSInitialize(IntPtr hRgssDll);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr RGSSDefineModule(IntPtr moduleName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr RGSSDefineModuleFunction(IntPtr module, IntPtr funcName, IntPtr funcAddress, int argcount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr OnEnemiesInfoDown(IntPtr obj, IntPtr dataString, uint len);

        public IntPtr onEnemiesInfoDown(IntPtr obj, IntPtr dataString, uint len)
        {
            len = (len - 1) / 2;
            // Console.WriteLine(len);

            IntPtr datas = new IntPtr((dataString.ToInt32() - 1) / 2);

            enemies = new List<Enemy>();

            for (int i = 0; i < len; i++)
            {
                IntPtr data = new IntPtr(Marshal.ReadInt32(IntPtr.Add(datas, i * 4)));
                int id = Marshal.ReadInt32(data);
                if (id == 0) continue;
                string name = Util.readString(IntPtr.Add(data, 4));
                if (name.Length == 0) continue;
                Enemy enemy = new Enemy();
                enemy.name = name;
                string special = Util.readString(IntPtr.Add(data, 8));
                if (special.StartsWith(";")) special = special.Substring(1);
                enemy.id = id;
                enemy.special = special;
                enemy.maxhp = Marshal.ReadInt32(IntPtr.Add(data, 20));
                enemy.maxsp = Marshal.ReadInt32(IntPtr.Add(data, 24));
                enemy.strength = Marshal.ReadInt32(IntPtr.Add(data, 28));
                enemy.dexterity = Marshal.ReadInt32(IntPtr.Add(data, 32));
                enemy.speed = Marshal.ReadInt32(IntPtr.Add(data, 36));
                enemy.magic = Marshal.ReadInt32(IntPtr.Add(data, 40));
                enemy.atk = Marshal.ReadInt32(IntPtr.Add(data, 44));
                enemy.def = Marshal.ReadInt32(IntPtr.Add(data, 48));
                enemy.mdef = Marshal.ReadInt32(IntPtr.Add(data, 52));
                enemy.dodge = Marshal.ReadInt32(IntPtr.Add(data, 56));
                enemy.experience = Marshal.ReadInt32(IntPtr.Add(data, 60));
                enemy.money = Marshal.ReadInt32(IntPtr.Add(data, 64));

                // Console.WriteLine(enemy.ToString());

                // Console.WriteLine("No." + id + ":" + name + "," + animation_name + "," + animation_hue + "," + position + "," + frame_max);

                enemies.Add(enemy);
            }

            return new IntPtr(4);
        }

        private string rxdataPath;
        private DllInvoke dll;

        private int id;
        private RGSSEval eval;
        private List<Enemy> enemies;

        private OnEnemiesInfoDown infoDown;

        public RGSSReader()
        {
            infoDown = onEnemiesInfoDown;
            rxdataPath = null;
            id = 0;

            dll = new DllInvoke("rgss.dll");
            ((RGSSInitialize)dll.Invoke("RGSSInitialize", typeof(RGSSInitialize)))(dll.getLib());

            IntPtr rb_define_module = IntPtr.Add(dll.getLib(), 0x20400);
            RGSSDefineModule rbDefineModule =
                (RGSSDefineModule)Marshal.GetDelegateForFunctionPointer(rb_define_module, typeof(RGSSDefineModule));

            IntPtr fux2 = rbDefineModule(Marshal.StringToCoTaskMemAnsi("Fux2"));

            IntPtr rb_define_module_function = IntPtr.Add(dll.getLib(), 0x20B50);
            RGSSDefineModuleFunction rbDefineModuleFunction =
                (RGSSDefineModuleFunction)Marshal.GetDelegateForFunctionPointer(rb_define_module_function,
                    typeof(RGSSDefineModuleFunction));

            rbDefineModuleFunction(fux2, Marshal.StringToCoTaskMemAnsi("sendEnemiesData"),
                Marshal.GetFunctionPointerForDelegate(infoDown), 2);

            eval = (RGSSEval)dll.Invoke("RGSSEval", typeof(RGSSEval));

        }

        public bool setPath(string _rxdataPath)
        {
            if (!File.Exists(_rxdataPath))
            {
                MessageBox.Show("Enemies.rxdata不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string systemPath = Path.Combine(Path.GetDirectoryName(_rxdataPath), "System.rxdata");
            if (!File.Exists(systemPath))
            {
                MessageBox.Show("System.rxdata不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // copy
            try
            {
                File.Delete("Enemies.rxdata");
                File.Copy(_rxdataPath, "Enemies.rxdata", true);
                File.SetAttributes("Enemies.rxdata", FileAttributes.Hidden | FileAttributes.System);
                File.Delete("System.rxdata");
                File.Copy(systemPath, "System.rxdata", true);
                File.SetAttributes("System.rxdata", FileAttributes.Hidden | FileAttributes.System);
                rxdataPath = _rxdataPath;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public void init()
        {
            enemies = null;
            if (rxdataPath != null)
            {
                try
                {
                    string code = string.Format(INIT, "Enemies.rxdata", "System.rxdata");
                    eval(Marshal.StringToCoTaskMemAnsi(code));
                    eval(Marshal.StringToCoTaskMemAnsi(GET));
                }
                catch (Exception e)
                {
                    enemies = null;
                    Console.WriteLine(e);
                }
            }
        }

        public List<Enemy> GetEnemies()
        {
            return enemies;
        }

        private const string INIT = @"
$mon = load_data(""{0}"")
$ele = load_data(""{1}"")
$ele_len = $ele.elements.size
$ele_names = []
class Table
    def x2arr
        ret = []
        xsize.times{{|i| ret << self[i]}}
        ret
    end
end
";

        private const string GET = @"
cinfos = $mon.map do |a|
    if a
        ele = a.element_ranks.x2arr
        elenames = []
        $ele_len.times{|i| ele[i]>0&&ele[i]!=3&&(elenames<<$ele.elements[i])}
        $ele_names[a.id] = elenames.inject(''){|d,b| ""#{d};#{b}""}
        [a.id,a.name,$ele_names[a.id],a.battler_name,a.battler_hue,a.maxhp,a.maxsp,a.str,a.dex,a.agi,a.int,a.atk,a.pdef,a.mdef,a.eva,a.exp,a.gold].pack(""lp3l*"")
    else
        ([0]*17).pack(""L*"")
    end
end
cinfos = cinfos.pack(""p*"")
ptr = [cinfos].pack(""p"").unpack(""L"")[0]
Fux2.sendEnemiesData(ptr,$mon.size)
";

    }



}
