using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncAnimation : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Start()
    {
        MoveAnimation();
    }

    private void OnMouseDown()
    {
        SquizeAnimation();
    }

    private async void MoveAnimation()
    {
        while (transform.position.y < 3)
        {
            transform.position += new Vector3(0, 1) * speed * Time.deltaTime;
            await UniTask.WaitForEndOfFrame(this);
        }

        while (transform.position.y > -3)
        {
            transform.position += new Vector3(0, -1) * speed * Time.deltaTime;
            await UniTask.WaitForEndOfFrame(this);
        }

        MoveAnimation();
    }

    private async void SquizeAnimation()
    {
        while (transform.localScale.x > 0.4f)
        {
            transform.localScale -= Vector3.one * speed * Time.deltaTime;
            await UniTask.WaitForEndOfFrame(this);
        }

        while (transform.localScale.x < 1)
        {
            transform.localScale += Vector3.one * speed * Time.deltaTime;
            await UniTask.WaitForEndOfFrame(this);
        }

        transform.localScale = Vector3.one;
    }
}
