using System.Collections.Generic;

namespace HardwareKiller.Models
{
    public class Graph
    {
        private readonly int _w, _h;
        private readonly (int dx, int dy)[] _dirs = { (1,0),(-1,0),(0,1),(0,-1) };

        public Graph(int width, int height)
        {
            _w = width; _h = height;
        }

        public List<(int X,int Y)> ShortestPath(int sx, int sy, int tx, int ty)
        {
            var q = new Queue<(int x,int y)>();
            var prev = new (int x,int y)?[_w, _h];
            q.Enqueue((sx, sy));
            prev[sx, sy] = (sx, sy);

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                if (u.x == tx && u.y == ty) break;
                foreach (var (dx, dy) in _dirs)
                {
                    int nx = u.x + dx, ny = u.y + dy;
                    if (nx<0||nx>=_w||ny<0||ny>=_h) continue;
                    if (prev[nx,ny] != null) continue;
                    prev[nx,ny] = u;
                    q.Enqueue((nx,ny));
                }
            }

            var path = new List<(int X,int Y)>();
            if (prev[tx,ty] == null) return path;
            var cur = (tx, ty);
            while (cur != prev[cur.Item1,cur.Item2].Value)
            {
                path.Add(cur);
                cur = prev[cur.Item1,cur.Item2].Value;
            }
            path.Add((sx, sy));
            path.Reverse();
            return path;
        }
    }
}