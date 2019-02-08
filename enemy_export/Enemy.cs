using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace enemy_export
{
    class Enemy
    {
        public string name;

        public override string ToString()
        {
            return string.Format("Name: {0}, Maxhp: {1}, Maxsp: {2}, Strength: {3}, Dexterity: {4}, Speed: {5}," +
                                 " Magic: {6}, Atk: {7}, Def: {8}, Mdef: {9}, Dodge: {10}, Money: {11}, Experience: {12}," +
                                 " Special: {13}", name, maxhp, maxsp, strength, dexterity, speed, magic, 
                                 atk, def, mdef, dodge, money, experience, special);
        }

        public string toJS()
        {
            return string.Format("'exEnemy{0}': {{'name': '{1}', 'hp': {2}, 'atk': {3}, 'def': {4}, 'money': {5}," +
                                 " 'experience': {6}, 'point': 0, 'special': {7}{8}{9}}},\n",
                                 id, name, maxhp, atk, def, money, experience, getSpecial(),
                                 special.Length==0?"":" /*"+special+"*/", extra());
        }

        public string getSpecial()
        {
            string[] specials =
                {
                    "", "先攻", "魔攻", "坚固", "", "", "连击", "破甲", "反击", "净化", "模仿",
                    "吸血", "中毒", "衰弱", "诅咒", "领域", "夹击", "仇恨", "阻击", "自爆", "无敌",
                    "退化", "固伤", "重生"
                }
            ;
            List<int> list = new List<int>();
            for (int i = 0; i < specials.Length; i++)
            {
                if (specials[i].Length > 0 && special.Contains(specials[i]))
                {
                    list.Add(i);
                }
            }
            if (list.Count == 0) return "0";
            if (list.Count == 1) return Convert.ToString(list[0]);
            return "[" + string.Join(",", list) + "]";
        }

        public string extra()
        {
            string content = "";
            if (special.Contains("连击"))
                content += ", 'n': " + dodge; // 连击由回避修正决定
            if (special.Contains("领域"))
                content += ", 'value': " + maxsp; // maxsp决定数值
            if (special.Contains("阻击")) // 阻击： xxx:/yy/
            {
                int index = name.IndexOf(":/");
                if (index >= 0)
                {
                    int index2 = name.IndexOf("/", index + 2);
                    if (index2 >= 0)
                    {
                        content += ", 'value': " + name.Substring(index + 2, index2 - index - 2);
                    }
                }
            }
            return content;
        }

        public int id;
        public int maxhp;
        public int maxsp;
        public int strength;
        public int dexterity; // 灵巧
        public int speed;
        public int magic; // 魔力
        public int atk;
        public int def;
        public int mdef;
        public int dodge; // 回避修正
        public int money;
        public int experience;
        public string special; // 技能
    }
}