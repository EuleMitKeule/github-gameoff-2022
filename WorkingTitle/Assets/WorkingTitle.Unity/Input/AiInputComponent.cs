using System;
using UnityEngine;
using AStar;

namespace WorkingTitle.Unity.Input
{
    public class AiInputComponent : InputComponent
    {
        Grid2D Grid2d { get; set; }

        void Awake()
        {
            Grid2d = GetComponent<Grid2D>();
        }

        void Update()
        {
            throw new NotImplementedException();
        }
    }
}