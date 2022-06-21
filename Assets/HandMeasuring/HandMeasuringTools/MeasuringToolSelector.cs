using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace HKT
{
    /// <summary>
    /// ハンドメジャーツールを切り替えるためのクラス
    /// 参考動画：https://twitter.com/hi_rom_/status/1267544392962699264
    /// </summary>
    /// 
    
    public class MeasuringToolSelector : MonoBehaviour
    {
        public static bool onehandActive;
        public static bool twohandsActive;
        public static bool protractorActive;

        public enum MeasuringTool
        {
            OneHandRuler = 0,
            TwoHandsRuler,
            HandProtractor
        }

        [SerializeField]
        private static List<GameObject> tools = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            onehandActive = false;
            twohandsActive = false;
            protractorActive = false;

            UseOneHandRuler();
            Measure.measureParameter = null;
        }

        public void UseOneHandRuler()
        {
            onehandActive = true;
            twohandsActive = false;
            protractorActive = false;

            Measure.measureParameter = "onehand";

            foreach (var tool in tools)
            {
                tool.SetActive(false);
            }
            tools[(int) MeasuringTool.OneHandRuler].SetActive(true);
        }

        public void UseTwoHandsRuler()
        {
            onehandActive = false;
            twohandsActive = true;
            protractorActive = false;

            Measure.measureParameter = "twohands";

            foreach (var tool in tools)
            {
                tool.SetActive(false);
            }
            tools[(int) MeasuringTool.TwoHandsRuler].SetActive(true);
        }

        public void UseHandProtractor()
        {
            onehandActive = false;
            twohandsActive = false;
            protractorActive = true;

            foreach (var tool in tools)
            {
                tool.SetActive(false);
            }
            tools[(int) MeasuringTool.HandProtractor].SetActive(true);
        }
    }
}
