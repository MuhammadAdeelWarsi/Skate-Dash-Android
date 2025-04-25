using DG.Tweening;
using UnityEngine;


namespace CustomControllers
{
    //Responsible for handling DOTween's play and stop methods in customized way.
    //Provides single implemented method to play and stop tweens in order to keep track of all custom tweens.
    //Designed especially for UIs to centrally animate UI elements using centralized and organized methods.
    public class TweenController
    {
        private TweenScale tweenScale;
        private TweenMove tweenMove;

        public TweenScale ScaleTweener => tweenScale;
        public TweenMove MoveTweener => tweenMove;

        public TweenController()
        {
            tweenScale = new TweenScale();
            tweenMove = new TweenMove();
        }

        #region SCALE
        public class TweenScale
        {
            public Tweener PlayScaleTween(Transform targetUI, float targetScale, float duration, Ease easeType)
            {
                Tweener tweener = targetUI.DOScale(targetScale, duration).SetEase(easeType);
                return tweener;
            }
            public Tweener PlayScaleTween(Transform targetUI, float targetScale, float duration, Ease easeType, int loopAmount, LoopType loopType)
            {
                Tweener tweener = targetUI.DOScale(targetScale, duration).SetEase(easeType).SetLoops(loopAmount, loopType);
                return tweener;
            }
        }
        #endregion

        #region MOVE
        public class TweenMove
        {
            public Tweener PlayLocalMoveTween(Transform targetUI, Vector3 targetLocalPosition, float duration, Ease easeType)
            {
                Tweener tweener = targetUI.DOLocalMove(targetLocalPosition, duration).SetEase(easeType);
                return tweener;
            }
            public Tweener PlayLocalMoveXTween(Transform targetUI, float targetPosition, float duration, Ease easeType)
            {
                Tweener tweener = targetUI.DOLocalMoveX(targetPosition, duration).SetEase(easeType);
                return tweener;
            }
            public Tweener PlayLocalMoveYTween(Transform targetUI, float targetPosition, float duration, Ease easeType)
            {
                Tweener tweener = targetUI.DOLocalMoveY(targetPosition, duration).SetEase(easeType);
                return tweener;
            }
            public Tweener PlayLocalMoveZTween(Transform targetUI, float targetPosition, float duration, Ease easeType)
            {
                Tweener tweener = targetUI.DOLocalMoveZ(targetPosition, duration).SetEase(easeType);
                return tweener;
            }
        }
        #endregion


        public void StopTween(Tweener targetTween)
        {
            DOTween.Kill(targetTween);
        }
        public void PauseTween(Tweener targetTween)
        {
            DOTween.Pause(targetTween);
        }
        public void ResumeTween(Tweener targetTween)
        {
            DOTween.Play(targetTween);
        }

        public void ResetUI()
        {
        }
    }
}