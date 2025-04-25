using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Android;


namespace CustomControllers
{
    public class UiTweener : MonoBehaviour
    {
        private Transform target;

        [SerializeField] TweenData[] tweensData;
        public TweenData[] TweensData => tweensData;


        private void Awake()
        {
            target = transform;
        }

        public Tweener PlayTween(TweenType tweenType, Action tweenStartCallback = null, Action tweenCompleteCallback = null)
        {
            TweenData tweenData = GetTweenData(tweenType);
            Tweener tweener = null;

            if(tweenData != null)
                CreateTween(tweenData, out tweener, tweenStartCallback, tweenCompleteCallback);

            else
                Debug.LogError("Target TweenType does not exist in current UI's TweenData.");

            //if (tweener != null && tweenCallback != null)
            //    tweenCallback.Invoke();

            return tweener;
        }
        public Tweener PlayTween(TweenType tweenType, TweenPurpose tweenPurpose, Action tweenStartCallback = null, Action tweenCompleteCallback = null)
        {
            TweenData tweenData = GetTweenData(tweenType, tweenPurpose);
            Tweener tweener = null;

            if (tweenData != null)
                CreateTween(tweenData, out tweener, tweenStartCallback, tweenCompleteCallback);

            else
                Debug.LogError("Target TweenType does not exist in current UI's TweenData.");

            //if (tweener != null && tweenCallback != null)
            //    tweenCallback.Invoke();

            return tweener;
        }

        private void CreateTween(TweenData tweenData, out Tweener tweener, Action tweenStartCallback = null, Action tweenCompleteCallback = null)
        {
            tweener = null;

            switch (tweenData.tweenType)
            {
                case TweenType.Scale:
                    if (tweenData.doPickFromCurrentValue)
                        tweenData.targetPoint = target.localScale;
                    if (tweenData.doPlayFromInitialValue)
                        target.localScale = tweenData.startPoint;
                    tweener = target.DOScale(tweenData.targetPoint, tweenData.duration).SetEase(tweenData.easeType);
                    break;

                case TweenType.LocalMove:
                    if (tweenData.doPickFromCurrentValue)
                        tweenData.targetPoint = target.localPosition;
                    if (tweenData.doPlayFromInitialValue)
                        target.localPosition = tweenData.startPoint;
                    tweener = target.DOLocalMove(tweenData.targetPoint, tweenData.duration).SetEase(tweenData.easeType);
                    break;

                case TweenType.LocalRotate:
                    if (tweenData.doPickFromCurrentValue)
                        tweenData.targetPoint = target.localEulerAngles;
                    if (tweenData.doPlayFromInitialValue)
                        target.localEulerAngles = tweenData.startPoint;
                    tweener = target.DOLocalRotate(tweenData.targetPoint, tweenData.duration).SetEase(tweenData.easeType);
                    break;
            }

            if(tweener != null)
            {
                if(tweenData.hasDelay)
                    tweener.SetDelay(tweenData.delay);

                if (tweenStartCallback != null)
                    tweener.OnStart(() => { tweenStartCallback.Invoke(); });

                if (tweenData.doLoop && tweener != null)
                    tweener.SetLoops(tweenData.loopAmount, tweenData.loopType);

                if (tweenData.doIgnoreTimeScale)
                    tweener.SetUpdate(true);

                if (tweenCompleteCallback != null)
                    tweener.OnComplete(() => { tweenCompleteCallback.Invoke(); });
            }
        }

        private TweenData GetTweenData(TweenType tweenType)
        {
            TweenData tweenData = tweensData.First(tween => tween.tweenType == tweenType);
            return tweenData;
        }
        private TweenData GetTweenData(TweenType tweenType, TweenPurpose tweenPurpose)
        {
            TweenData tweenData = tweensData.First(tween => tween.tweenType == tweenType && tween.tweenPurpose == tweenPurpose);
            return tweenData;
        }


        #region TWEEN INFO
        [System.Serializable]
        public class TweenData
        {
            public TweenType tweenType;
            public TweenPurpose tweenPurpose;

            [Space(5)] public Vector3 startPoint;
            public Vector3 targetPoint;

            [Space(5)] public float duration = 1f;
            public Ease easeType = Ease.Linear;

            [Space(5)] public bool hasDelay = false;
            public float delay = 0f;

            [Space(5)] public bool doPlayFromInitialValue = true;
            [Tooltip("If enabled, make target value equal to current value")] public bool doPickFromCurrentValue = false;

            [Space(5)] public bool doLoop = false;
            public int loopAmount = 0;
            public LoopType loopType = LoopType.Restart;

            [Space(5)] public bool doIgnoreTimeScale = false;
        }

        public enum TweenType
        {
            Scale,

            Move,
            LocalMove,

            Rotate,
            LocalRotate
        }
        public enum TweenPurpose
        {
            Initialize, //For initializing or initial animation
            Emphasize,  //For bringing attractiveness and focus
            Hide        //For hiding or disabling
        }
        #endregion
    }
}