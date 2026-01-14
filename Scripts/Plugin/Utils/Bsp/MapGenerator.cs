using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

namespace Utils.Bsp
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private float _minimumDivideRate;
        [SerializeField] private float _maximumDivideRate;
        [SerializeField] private GameObject _line;
        [SerializeField] private GameObject _map;
        [SerializeField] private GameObject _roomLine;
        [SerializeField] private int _maximumDepth;

        private void Start()
        {
            Node root = new Node(new RectInt(0, 0, _mapSize.x, _mapSize.y));
            DrawMap(0, 0);
            Divide(root, 0);
            GenerateRoom(root, 0);
            GenerateLoad(root, 0);
        }

        private void DrawMap(int x, int y)
        {
            LineRenderer lineRenderer = Instantiate(_map).GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector2(x, y) - _mapSize / 2);
            lineRenderer.SetPosition(1, new Vector2(x + _mapSize.x, y) - _mapSize / 2);
            lineRenderer.SetPosition(2, new Vector2(x + _mapSize.x, y + _mapSize.y) - _mapSize / 2);
            lineRenderer.SetPosition(3, new Vector2(x, y + _mapSize.y) - _mapSize / 2);
        }

        private void Divide(Node tree, int n)
        {
            if (n == _maximumDepth) return;

            int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
            int split = Mathf.RoundToInt(UnityEngine.Random.Range(maxLength * _minimumDivideRate, maxLength * _maximumDivideRate));

            if(tree.nodeRect.width >= tree.nodeRect.height)
            {
                tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
                tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));

                //DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
            }
            else
            {
                tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
                tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
                //DrawLine(new Vector2(tree.nodeRect.x, tree.nodeRect.y + split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y + split));
            }

            tree.leftNode.parNode = tree;
            tree.rightNode.parNode = tree;
            Divide(tree.leftNode, n + 1);
            Divide(tree.rightNode, n + 1);
        }

        private void DrawLine(Vector2 from, Vector2 to)
        {
            LineRenderer lineRenderer = Instantiate(_line).GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, from - _mapSize / 2);
            lineRenderer.SetPosition(1, to - _mapSize / 2);
        }

        private RectInt GenerateRoom(Node tree, int n)
        {
            RectInt rect;
            if(n == _maximumDepth)
            {
                rect = tree.nodeRect;
                int width = Random.Range(rect.width / 2, rect.width - 1);
                int height = Random.Range(rect.height / 2, rect.height - 1);
                int x = rect.x + Random.Range(1, rect.width - width);
                int y = rect.y + Random.Range(1, rect.height - height);
                rect = new RectInt(x, y, width, height);
                DrawRectangle(rect);
            }
            else
            {
                tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
                tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
                rect = tree.leftNode.roomRect;
            }

            return rect;
        }

        private void DrawRectangle(RectInt rect)
        {
            LineRenderer lineRenderer = Instantiate(_roomLine).GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector2(rect.x, rect.y) - _mapSize / 2);
            lineRenderer.SetPosition(1, new Vector2(rect.x + rect.width, rect.y) - _mapSize / 2);
            lineRenderer.SetPosition(2, new Vector2(rect.x + rect.width, rect.y + rect.height) - _mapSize / 2);
            lineRenderer.SetPosition(3, new Vector2(rect.x, rect.y + rect.height) - _mapSize / 2);
        }

        private void GenerateLoad(Node tree, int n)
        {
            if (n == _maximumDepth) return;

            Vector2Int leftNodeCenter = tree.leftNode.center;
            Vector2Int rightNodeCenter = tree.rightNode.center;

            DrawLine(new Vector2(leftNodeCenter.x, leftNodeCenter.y), new Vector2(rightNodeCenter.x, leftNodeCenter.y));
            DrawLine(new Vector2(rightNodeCenter.x, leftNodeCenter.y), new Vector2(rightNodeCenter.x, rightNodeCenter.y));

            GenerateLoad(tree.leftNode, n + 1);
            GenerateLoad(tree.rightNode, n + 1);
        }
    }
}
