using Infrastructure;
using Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using UiElements;
using UnityEngine;

namespace DraggableImages
{
    public class GalleryImagesController : MonoBehaviour
    {
        [SerializeField] private DraggableImage _draggableImagePrefab;
        [SerializeField] private RectTransform _galleryImageContainer;
        [SerializeField] private RectTransform _galleryImageCreationPosition;
        [SerializeField] private Canvas _mainCanvas;
        private Func<PollPhase> _getPollPhaseFunc;
        private List<DraggableImage> _draggableImages = new();

        public void Initialize(Func<PollPhase> getPollPhaseFunc)
        {
            _getPollPhaseFunc = getPollPhaseFunc;
        }

        public void GetImageFromGallery()
        {
            NativeGallery.GetImageFromGallery(GalleryImageCallback);
        }

        private void GalleryImageCallback(string path)
        {
            LoadImageFromGallery(path);
        }

        private async void LoadImageFromGallery(string path)
        {
            try
            {
                var texturre = await NativeGallery.LoadImageAtPathAsync(path);
                if (texturre == null || _getPollPhaseFunc() == PollPhase.Results)
                {
                    return;
                }
                var newGalleryImage = ObjectPool.Instance.Borrow(_draggableImagePrefab, _galleryImageContainer).GetComponent<DraggableImage>();
                newGalleryImage.transform.position = _galleryImageCreationPosition.transform.position;
                newGalleryImage.ShowImage(texturre, _mainCanvas.scaleFactor);
                _draggableImages.Add(newGalleryImage);
            } catch
            {
                Debug.LogError("Failed loading image from Gallery!");
            }
        }

        public void HideAll()
        {
            for (var i = 0; i < _draggableImages.Count; i++)
            {
                var draggableImage = _draggableImages[i];
                draggableImage.AnimateToFullTransparency();
            }
        }
    }
}