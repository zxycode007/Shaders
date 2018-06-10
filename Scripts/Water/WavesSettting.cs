using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Lin
{
    public class WavesSettting : MonoBehaviour
    {
        [Serializable]
        public class Wave
        {
            public float Amplitude; // 振幅
            public float Angle;     // 角度
            public float Frequency; // 频率，单位大小内，波浪个数
            public float Velocity;  // 速度
            public int Steepness;   // 陡度
            public bool Excluded;
            [HideInInspector]
            public Vector2 Direction;   // 方向
        }

        public Wave[] Waves;    // 波浪，可以是多个
        private float _time;    // 时间增量
        public Vector2Int _grid = new Vector2Int();
        [HideInInspector]
        public float[] _vertY;  // 顶点的Y值


        public void UpdateWave()
        {
            _time += Time.deltaTime;
            foreach (Wave wave in Waves)
            {
                // 根据输入的的角度【0,360】获取当前水方向
                wave.Direction = new Vector2(Mathf.Cos(wave.Angle * 1f / 180f * Mathf.PI), Mathf.Sin(wave.Angle * 1f / 180f * Mathf.PI));
            }

            Vector2 invGrid = new Vector2(1f / _grid.x, 1f / _grid.y);
            for (int i = 0; i < _grid.x; i++)
            {
                for (int j = 0; j < _grid.y; j++)
                {
                    int index = j * _grid.x + i;

                    // 2π = 360度，值域为【0,1】时，求当前位置的角度
                    float normX = i * invGrid.x * Mathf.PI * 2f;
                    float normY = j * invGrid.y * Mathf.PI * 2f;
                    _vertY[index] = 0f;
                    foreach (Wave wave in Waves)
                    {
                        if (wave.Excluded) continue;
                        // val = (水方向.x * 当前点单位角度x + 水方向.y * 当前点单位角度y） * 频率 + 速度
                        float val = (wave.Direction.x * normX + wave.Direction.y * normY) * wave.Frequency + _time * wave.Velocity;
                        // 最终值 = pow((sin(val) + 1) *0.5, 坡度) * 振幅
                        _vertY[index] += Mathf.Pow((Mathf.Sin(val) + 1f) * 0.5f, wave.Steepness) * wave.Amplitude;
                    }
                }
            }
        }
    }
}