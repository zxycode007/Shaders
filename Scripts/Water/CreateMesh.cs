using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Lin
{
    struct MeshStruct
    {
        public int[] Triangles;
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector4[] Tangents;
        public Vector2[] UV;
        public Color32[] Colors32;
    }

    public struct Vector2Int
    {
        public int x;
        public int y;
        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2Int(Vector2 value)
        {
            x = (int)value.x;
            y = (int)value.y;
        }
        public static implicit operator Vector2Int(Vector2 value)
        {
            return new Vector2Int((int)value.x, (int)value.y);
        }
        public override string ToString()
        {
            return "{" + x + ", " + y + "}";
        }
    }
    public class CreateMesh : MonoBehaviour
    {
        public int _quality = 20;
        // 面片最大大小  
        public Vector2 _size = new Vector2(1000, 1000);

        // 表格个数  
        private Vector2Int _grid = new Vector2Int();
        // 单元格大小  
        private float _nodeSize = 0f;

        private Mesh _mesh;
        // 网格结构体  
        private MeshStruct _meshStruct;
        private MeshFilter _meshFilter;

        private WavesSettting anima;

        void Start()
        {
            _mesh = new Mesh();
            _mesh.name = "DynamicWaterMesh";
            _meshFilter = gameObject.GetComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;

            _quality = Mathf.Clamp(_quality, 4, 256);
            // 根据质量计算单元格大小  
            _nodeSize = Mathf.Max(_size.x, _size.y) / _quality;
            // 根据【单元格大小】计算【格子个数】  
            _grid.x = Mathf.RoundToInt(_size.x / _nodeSize) + 1;
            _grid.y = Mathf.RoundToInt(_size.y / _nodeSize) + 1;
            // 创建平面mesh  
            _mesh.MarkDynamic();
            AllocateMeshArrays();
            CreateMeshGrid();
            AssignMesh();
            _meshFilter.mesh = _mesh;
            // 初始化顶点运动  
            anima = gameObject.GetComponent<WavesSettting>();
            anima._grid = _grid;
            anima._vertY = new float[_grid.x * _grid.y];
        }

        public void Update()
        {
            anima.UpdateWave();
            UpdateMesh(ref anima._vertY);
        }

        private void AllocateMeshArrays()
        {
            int numVertices = _grid.x * _grid.y;
            _meshStruct.Vertices = new Vector3[numVertices];
            _meshStruct.Normals = new Vector3[numVertices];
            _meshStruct.Tangents = new Vector4[numVertices];
            _meshStruct.Colors32 = new Color32[numVertices];
            _meshStruct.UV = new Vector2[numVertices];
            _meshStruct.Triangles = new int[((_grid.x - 1) * (_grid.y - 1)) * 2 * 3];
        }

        private void CreateMeshGrid()
        {
            float uvStepXInit = 1f / (_size.x / _nodeSize);
            float uvStepYInit = 1f / (_size.y / _nodeSize);
            Color32 colorOne = new Color32(255, 255, 255, 255);
            Vector3 up = Vector3.up;
            bool setTangents = true;

            Vector4 tangent = new Vector4(1f, 0f, 0f, 1f);

            int k = 0;

            for (int j = 0; j < _grid.y; j++)
            {
                for (int i = 0; i < _grid.x; i++)
                {
                    int index = j * _grid.x + i;

                    // Set vertices  
                    _meshStruct.Vertices[index].x = i * _nodeSize;
                    _meshStruct.Vertices[index].y = 0f;
                    _meshStruct.Vertices[index].z = j * _nodeSize;

                    // Set triangles  
                    if (j < _grid.y - 1 && i < _grid.x - 1)
                    {
                        _meshStruct.Triangles[k + 0] = (j * _grid.x) + i;
                        _meshStruct.Triangles[k + 1] = ((j + 1) * _grid.x) + i;
                        _meshStruct.Triangles[k + 2] = (j * _grid.x) + i + 1;

                        _meshStruct.Triangles[k + 3] = ((j + 1) * _grid.x) + i;
                        _meshStruct.Triangles[k + 4] = ((j + 1) * _grid.x) + i + 1;
                        _meshStruct.Triangles[k + 5] = (j * _grid.x) + i + 1;

                        k += 6;
                    }

                    // Set UV  
                    float uvStepX = uvStepXInit;
                    float uvStepY = uvStepYInit;

                    _meshStruct.UV[index].x = i * uvStepX;
                    _meshStruct.UV[index].y = j * uvStepY;

                    // Set colors  
                    _meshStruct.Colors32[index] = colorOne;

                    // Set normals  
                    _meshStruct.Normals[index] = up;

                    if (setTangents)
                    {
                        // set tangents  
                        _meshStruct.Tangents[index] = tangent;
                    }

                    // fix stretching  
                    float delta;

                    if (_meshStruct.Vertices[index].x > _size.x)
                    {
                        delta = (_size.x - _meshStruct.Vertices[index].x) / _nodeSize;
                        _meshStruct.UV[index].x -= uvStepX * delta;

                        _meshStruct.Vertices[index].x = _size.x;
                    }

                    if (_meshStruct.Vertices[index].z > _size.y)
                    {
                        delta = (_size.y - _meshStruct.Vertices[index].z) / _nodeSize;
                        _meshStruct.UV[index].y -= uvStepY * delta;

                        _meshStruct.Vertices[index].z = _size.y;
                    }

                    if (_size.x - _meshStruct.Vertices[index].x < _nodeSize)
                    {
                        delta = (_size.x - _meshStruct.Vertices[index].x) / _nodeSize;
                        _meshStruct.UV[index].x += uvStepX * delta;

                        _meshStruct.Vertices[index].x = _size.x;
                    }

                    if (_size.y - _meshStruct.Vertices[index].z < _nodeSize)
                    {
                        delta = (_size.y - _meshStruct.Vertices[index].z) / _nodeSize;
                        _meshStruct.UV[index].y += uvStepY * delta;

                        _meshStruct.Vertices[index].z = _size.y;
                    }
                }
            }
        }

        private void AssignMesh()
        {
            _mesh.vertices = _meshStruct.Vertices;
            _mesh.normals = _meshStruct.Normals;
            _mesh.tangents = _meshStruct.Tangents;

            _mesh.uv = _meshStruct.UV;
            _mesh.colors32 = _meshStruct.Colors32;
            _mesh.triangles = _meshStruct.Triangles;

            _mesh.RecalculateBounds();

            // Freeing the memory  
            _meshStruct.Tangents = null;
            _meshStruct.Triangles = null;
            _meshStruct.UV = null;
            _meshStruct.Colors32 = null;
        }

        public void UpdateMesh(ref float[] field)
        {
            for (int j = 0; j < _grid.y; j++)
            {
                int index = j * _grid.x;
                for (int i = 0; i < _grid.x; i++)
                {
                    _meshStruct.Vertices[index].y = field[index];
                    index++;
                }
            }
            _mesh.vertices = _meshStruct.Vertices;
            return;
        }
    }
}