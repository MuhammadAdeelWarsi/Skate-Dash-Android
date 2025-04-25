using System;
using UnityEngine;


namespace CustomControllers
{
    public class ScreenFitController : MonoBehaviour
    {
        private float currentAspectRatio, referenceAspectRatio, factor;

        [Header("---Screen Size---")]
        [SerializeField] int referenceScreenWidth;
        [SerializeField] int referenceScreenHeight;

        [Header("---Screen Zoom---")]
        [SerializeField] int referenceOrthographicSize;
        [SerializeField] Camera targetCamera;

        [Header("---Objects Scale---")]
        [SerializeField] Transform[] scalableObjectsTransforms;

        [Header("---Objects Position---")]
        [SerializeField] ObjectsRepositioningDetails[] objectsRepositioningDetails;


        private void Awake()
        {
            currentAspectRatio = (float)Screen.width / (float)Screen.height;
            referenceAspectRatio = (float)referenceScreenWidth / (float)referenceScreenHeight;
            factor = currentAspectRatio / referenceAspectRatio;

            AdjustCameraSize();

            if(scalableObjectsTransforms.Length > 0)
                AdjustObjectScale();

            if (objectsRepositioningDetails.Length > 0)
                AdjustObjectPosition();
        }

        private void AdjustCameraSize()
        {
            targetCamera.orthographicSize = referenceOrthographicSize / factor;
        }
        private void AdjustObjectScale()
        {
            foreach(Transform targetObjectTransform in scalableObjectsTransforms)
            {
                targetObjectTransform.localScale /= factor;
            }
        }
        private void AdjustObjectPosition()
        {
            foreach(ObjectsRepositioningDetails objectsRepositioningDetail in objectsRepositioningDetails)
            {
                if (objectsRepositioningDetail.UseGlobal)
                {
                    objectsRepositioningDetail.TargetTransform.position = objectsRepositioningDetail.ReferenceTransform.position + objectsRepositioningDetail.PositionOffset;
                }
                else
                {
                    objectsRepositioningDetail.TargetTransform.localPosition = objectsRepositioningDetail.ReferenceTransform.localPosition + objectsRepositioningDetail.PositionOffset;
                }
            }
        }
        
        /// <summary>
        /// Adjusts the value of a floating field with respect to a factor difference between reference and current aspect ratios.
        /// </summary>
        public void AdjustFieldValue(ref float targetedField)
        {
            targetedField /= factor;
        }

        /// <summary>
        /// Adjusts the value of an integer field with respect to a factor difference between reference and current aspect ratios.
        /// </summary>
        public void AdjustFieldValue(ref int targetedField)
        {
            float tempFieldValue = targetedField;
            tempFieldValue /= factor;

            targetedField = factor < 1 ? Mathf.CeilToInt(tempFieldValue) : Mathf.FloorToInt(tempFieldValue);
        }
        

        #region SUB-CLASSES
        [Serializable]
        public class ObjectsRepositioningDetails
        {
            [SerializeField] Transform targetTransform;
            [SerializeField] Transform referenceTransform;
            [SerializeField] bool useGlobal;
            [SerializeField] Vector3 positionOffset;

            public Transform TargetTransform => targetTransform;
            public Transform ReferenceTransform => referenceTransform;
            public bool UseGlobal => useGlobal;
            public Vector3 PositionOffset => positionOffset;
        } 
        #endregion
    }
}