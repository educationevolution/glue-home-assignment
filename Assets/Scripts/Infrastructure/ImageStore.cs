using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class ImageStore
    {
        public Sprite LoadImage(string url)
        {
            return Resources.Load<Sprite>(url);
        }
    }
}