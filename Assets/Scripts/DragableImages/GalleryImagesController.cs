using Infrastructure;
using Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using UiElements;
using UnityEngine;

namespace DragableImages
{
    public class GalleryImagesController : MonoBehaviour
    {
        [SerializeField] private DragableImage _dragableImagePrefab;
        [SerializeField] private RectTransform _galleryImageContainer;
        [SerializeField] private RectTransform _galleryImageCreationPosition;
        private Func<PollPhase> _getPollPhaseFunc;
        private List<DragableImage> _dragableImages = new();

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
                var newGalleryImage = ObjectPool.Instance.Borrow(_dragableImagePrefab, _galleryImageContainer).GetComponent<DragableImage>();
                newGalleryImage.transform.position = _galleryImageCreationPosition.transform.position;
                newGalleryImage.ShowImage(texturre);
                _dragableImages.Add(newGalleryImage);
            } catch
            {
                Debug.LogError("Failed loading image from Gallery!");
            }
        }

        public void HideAll()
        {
            for (var i = 0; i < _dragableImages.Count; i++)
            {
                var dragableImage = _dragableImages[i];
                dragableImage.AnimateToFullTransparency();
            }
        }
    }
}