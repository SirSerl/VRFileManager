using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))] //It needs a rect component
public class VRUIItem : MonoBehaviour
{
    private BoxCollider boxCollider;
    private RectTransform rectTransform;

    private void OnEnable()
    {
        ValidateCollider();
    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    private void ValidateCollider()
    {
        rectTransform = GetComponent<RectTransform>();

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) //check if it has a boxCollider
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();//if not add one
        }

        boxCollider.size = rectTransform.sizeDelta;//Set the size to the size of the button
    }
}

