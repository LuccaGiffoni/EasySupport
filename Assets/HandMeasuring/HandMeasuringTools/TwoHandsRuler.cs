using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace MRTK_HKSample
{
    /// <summary>
    /// ハンド定規
    /// 参考動画：https://twitter.com/hi_rom_/status/1267100537578639363
    /// </summary>
    public class TwoHandsRuler : MonoBehaviour
    {
        [SerializeField]
        private TextMesh distanceText;

        // Sucess boxes
        [SerializeField] private TextMeshPro recordedText;

        [SerializeField] private LineRenderer line = default;

        private IMixedRealityHandJointService handJointService = null;
        private IMixedRealityDataProviderAccess dataProviderAccess = null;

        #region Properties

        public TextMesh DistanceText
        {
            get { return distanceText; }
            set { distanceText = value; }
        }

        public TextMeshPro RecordedText
        {
            get { return recordedText; }
            set { recordedText = value; }
        }

        public LineRenderer Line
        {
            get { return line; }
        }

        #endregion

        void Start()
        {
            handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
            if (handJointService == null)
            {
                Debug.LogError("Can't get IMixedRealityHandJointService.");
                return;
            }

            dataProviderAccess = CoreServices.InputSystem as IMixedRealityDataProviderAccess;
            if (dataProviderAccess == null)
            {
                Debug.LogError("Can't get IMixedRealityDataProviderAccess.");
                return;
            }

            Initialize();
        }

        public void Initialize()
        {
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
            distanceText.text = "0";
        }

        void Update()
        {
            // 左手
            var leftIndexTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Left);
            if (leftIndexTip == null)
            {
                Debug.Log("leftIndexTip is null.");
                return;
            }

            // 右手
            var rightIndexTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Right);
            if (rightIndexTip == null)
            {
                Debug.Log("rightIndexTip is null.");
                return;
            }

            // 線を描画
            line.SetPosition(0, leftIndexTip.position);
            line.SetPosition(1, rightIndexTip.position);
            line.startWidth = 0.001f;
            line.endWidth = 0.001f;

            // 距離を算出
            var distance = Vector3.Distance(leftIndexTip.position, rightIndexTip.position);
            // cmに変換
            distance = distance * 100;
            distanceText.text = distance.ToString("0.0");
            distanceText.transform.position = (leftIndexTip.position + rightIndexTip.position) / 2;
        }

        public void Record()
        {
            if (Measure.measureParameter == "twohands")
            {
                recordedText.text = distanceText.text + " cm";
            }
        }
    }
}