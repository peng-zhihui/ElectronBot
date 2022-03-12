using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
    public static class GraphicsUtility
    {
        private static Mesh m_quadMesh;

        static GraphicsUtility()
        {
            m_quadMesh = CreateQuad();
        }

        public static float GetScreenScale(Vector3 position, Camera camera)
        {
            float h = camera.pixelHeight;
      
            if (camera.orthographic)
            {

                return camera.orthographicSize * 2f / h * 90;
            }

            Transform transform = camera.transform;
            float distance = camera.stereoEnabled ?
                (position - transform.position).magnitude :
                Vector3.Dot(position - transform.position, transform.forward);

            float scale = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            return scale / h * 90;
        }

        public static Mesh CreateCube(Color color, Vector3 center, float scale, float cubeLength = 1, float cubeWidth = 1, float cubeHeight = 1)
        {
            cubeHeight *= scale;
            cubeWidth *= scale;
            cubeLength *= scale;

            Vector3 vertice_0 = center + new Vector3(-cubeLength * .5f, -cubeWidth * .5f, cubeHeight * .5f);
            Vector3 vertice_1 = center + new Vector3(cubeLength * .5f, -cubeWidth * .5f, cubeHeight * .5f);
            Vector3 vertice_2 = center + new Vector3(cubeLength * .5f, -cubeWidth * .5f, -cubeHeight * .5f);
            Vector3 vertice_3 = center + new Vector3(-cubeLength * .5f, -cubeWidth * .5f, -cubeHeight * .5f);
            Vector3 vertice_4 = center + new Vector3(-cubeLength * .5f, cubeWidth * .5f, cubeHeight * .5f);
            Vector3 vertice_5 = center + new Vector3(cubeLength * .5f, cubeWidth * .5f, cubeHeight * .5f);
            Vector3 vertice_6 = center + new Vector3(cubeLength * .5f, cubeWidth * .5f, -cubeHeight * .5f);
            Vector3 vertice_7 = center + new Vector3(-cubeLength * .5f, cubeWidth * .5f, -cubeHeight * .5f);
            Vector3[] vertices = new[]
            {
                // Bottom Polygon
                vertice_0, vertice_1, vertice_2, vertice_3,
                // Left Polygon
                vertice_7, vertice_4, vertice_0, vertice_3,
                // Front Polygon
                vertice_4, vertice_5, vertice_1, vertice_0,
                // Back Polygon
                vertice_6, vertice_7, vertice_3, vertice_2,
                // Right Polygon
                vertice_5, vertice_6, vertice_2, vertice_1,
                // Top Polygon
                vertice_7, vertice_6, vertice_5, vertice_4
            };

            int[] triangles = new[]
            {
                // Cube Bottom Side Triangles
                3, 1, 0,
                3, 2, 1,    
                // Cube Left Side Triangles
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                // Cube Front Side Triangles
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                // Cube Back Side Triangles
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                // Cube Rigth Side Triangles
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                // Cube Top Side Triangles
                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i] = color;
            }

            Mesh cubeMesh = new Mesh();
            cubeMesh.name = "cube";
            cubeMesh.vertices = vertices;
            cubeMesh.triangles = triangles;
            cubeMesh.colors = colors;
            cubeMesh.RecalculateNormals();
            return cubeMesh;
        }

        public static Mesh CreateQuad(float quadWidth = 1, float quadHeight = 1)
        {
            Vector3 vertice_0 = new Vector3(-quadWidth * .5f, -quadHeight * .5f, 0);
            Vector3 vertice_1 = new Vector3(quadWidth * .5f, -quadHeight * .5f, 0);
            Vector3 vertice_2 = new Vector3(-quadWidth * .5f, quadHeight * .5f, 0);
            Vector3 vertice_3 = new Vector3(quadWidth * .5f, quadHeight * .5f, 0);

            Vector3[] vertices = new[]
            {
                vertice_2, vertice_3, vertice_1, vertice_0,
            };

            int[] triangles = new[]
            {
                // Cube Bottom Side Triangles
                3, 1, 0,
                3, 2, 1,
            };

            Vector2[] uvs =
            {
                new Vector2(1, 0),
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            };

            Mesh quadMesh = new Mesh();
            quadMesh.name = "quad";
            quadMesh.vertices = vertices;
            quadMesh.triangles = triangles;
            quadMesh.uv = uvs;
            quadMesh.RecalculateNormals();
            return quadMesh;
        }

        public static Mesh CreateWireQuad(float quadWidth = 1, float quadHeight = 1)
        {
            Vector3 vertice_0 = new Vector3(-quadWidth * .5f, -quadHeight * .5f, 0);
            Vector3 vertice_1 = new Vector3(quadWidth * .5f, -quadHeight * .5f, 0);
            Vector3 vertice_2 = new Vector3(quadWidth * .5f, quadHeight * .5f, 0);
            Vector3 vertice_3 = new Vector3(-quadWidth * .5f, quadHeight * .5f, 0);
            
            Vector3[] vertices = new[]
            {
                vertice_0, vertice_1, vertice_2, vertice_3
            };

            Mesh quadMesh = new Mesh();
            quadMesh.vertices = vertices;
            quadMesh.SetIndices(new[] { 0, 1, 1, 2, 2, 3, 3, 0 }, MeshTopology.Lines, 0);
            
            return quadMesh;
        }

        public static Mesh CreateWireCubeMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new[]
            {
                new Vector3(-1, -1, -1),
                new Vector3(-1, -1,  1),
                new Vector3(-1,  1, -1),
                new Vector3(-1,  1,  1),
                new Vector3( 1, -1, -1),
                new Vector3( 1, -1,  1),
                new Vector3( 1,  1, -1),
                new Vector3( 1,  1,  1),
            };
            mesh.SetIndices(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 0, 2, 1, 3, 4, 6, 5, 7, 0, 4, 1, 5, 2, 6, 3, 7 }, MeshTopology.Lines, 0);
            return mesh;
        }

        public static Mesh CreateWireCircle(float radius = 1, int pointsCount = 64)
        {
            return CreateWireArc(Vector3.zero, radius, pointsCount, 0, Mathf.PI * 2);
        }

        public static Mesh CreateWireArc(Vector3 offset, float radius = 1, int pointsCount = 64, float fromAngle = 0, float toAngle = Mathf.PI * 2)
        {   
            Vector3[] vertices = new Vector3[pointsCount + 1];

            List<int> indices = new List<int>();
            for(int i = 0; i < pointsCount; ++i)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }

            float currentAngle = fromAngle;
            float deltaAngle = toAngle - fromAngle;
            float z = 0.0f;
            float x = Mathf.Cos(currentAngle) * radius;
            float y = Mathf.Sin(currentAngle) * radius;

            Vector3 prevPoint = new Vector3(x, y, z) + offset;
            for (int i = 0; i < pointsCount; i++)
            {
                vertices[i] = prevPoint;
                currentAngle += deltaAngle / pointsCount;
                x = Mathf.Cos(currentAngle) * radius;
                y = Mathf.Sin(currentAngle) * radius;
                Vector3 point = new Vector3(x, y, z) + offset;
                vertices[i + 1] = point;
                prevPoint = point;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            return mesh;
        }

        public static Mesh CreateWireCylinder(float radius = 1.0f, float length = 1.0f, int pointsCount = 8, float fromAngle = 0, float toAngle = Mathf.PI * 2)
        {
            Vector3[] vertices = new Vector3[pointsCount * 2];
            List<int> indices = new List<int>();
            for (int i = 0; i < vertices.Length; i += 2)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }

            float currentAngle = fromAngle;
            float deltaAngle = toAngle - fromAngle;
            float z = 0.0f;

            for (int i = 0; i < vertices.Length; i += 2)
            {
                float x = radius * Mathf.Cos(currentAngle);
                float y = radius * Mathf.Sin(currentAngle);
                Vector3 point = new Vector3(x, y, z);
                Vector3 point2 = new Vector3(x, y, z) + Vector3.forward * length;
                vertices[i] = point;
                vertices[i + 1] = point2;
                currentAngle += deltaAngle / pointsCount;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            return mesh;
        }

        public static void DrawMesh(CommandBuffer commandBuffer, Mesh mesh, Matrix4x4 transform, Material material, MaterialPropertyBlock propertyBlock)
        {
            commandBuffer.DrawMesh(mesh, transform, material, 0, 0, propertyBlock);
        }
      
        public static Mesh CreateCone(Color color, float scale)
        {
            int segmentsCount = 12;
            float size = 1.0f / 5;
            size *= scale;

            Vector3[] vertices = new Vector3[segmentsCount * 3 + 1];
            int[] triangles = new int[segmentsCount * 6];
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i] = color;
            }

            float radius = size / 2.6f;
            float height = size;
            float deltaAngle = Mathf.PI * 2.0f / segmentsCount;

            float y = -height;

            vertices[vertices.Length - 1] = new Vector3(0, -height, 0);
            for (int i = 0; i < segmentsCount; i++)
            {
                float angle = i * deltaAngle;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices[i] = new Vector3(x, y, z);
                vertices[segmentsCount + i] = new Vector3(0, 0.01f, 0);
                vertices[2 * segmentsCount + i] = vertices[i];
            }

            for (int i = 0; i < segmentsCount; i++)
            {
                triangles[i * 6] = i;
                triangles[i * 6 + 1] = segmentsCount + i;
                triangles[i * 6 + 2] = (i + 1) % segmentsCount;

                triangles[i * 6 + 3] = vertices.Length - 1;
                triangles[i * 6 + 4] = 2 * segmentsCount + i;
                triangles[i * 6 + 5] = 2 * segmentsCount + (i + 1) % segmentsCount;
            }

            Mesh cone = new Mesh();
            cone.name = "Cone";
            cone.vertices = vertices;
            cone.triangles = triangles;
            cone.colors = colors;

            return cone;
        }


    }
}
