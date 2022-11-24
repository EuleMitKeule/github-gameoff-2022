using Sirenix.OdinInspector;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map
{
    public class BoomBarrierComponent : SerializedMonoBehaviour
    {
        int EntityCount { get; set; }

        Animator Animator { get; set; }

        void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (EntityCount < 1)
            {
                Animator.SetTrigger("open");
            }
            EntityCount += 1;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            EntityCount -= 1;
            if (EntityCount < 1)
            {
                Animator.SetTrigger("close");
            }
        }
    }
}