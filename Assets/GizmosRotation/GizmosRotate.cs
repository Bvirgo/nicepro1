using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

    public class GizmosRotate : MonoSingletion<GizmosRotate>
    {
        public delegate void AxisEventHandler(EAxis axis, float startAngle);
        public delegate void RotateEventHandler(EAxis axis, Vector3 euler);

        public Camera RenderCamera;
        public event AxisEventHandler OnAxisChanged;
        public event RotateEventHandler OnRotateChanged;
        public event RotateEventHandler OnRotateEnd;

        public float SnapAngle = 5;
        public float RotateSpeed = 1;
        public bool EnableFreeRotate = false;

        public enum EAxis
        {
            None = -1,
            X,
            Y,
            Z,
            All
        }
        private List<int> m_pVtNums = new List<int>();
        private float m_fRadius = 2;

        private float m_fTheta = 0.1f;
        private GameObject m_pSphere = null;

        private GameObject m_pMeshGo = null;
        private Mesh m_pMesh;

        private GameObject m_pDynamicArcGo = null;
        private Mesh m_pDynamicArcMesh;

        private int m_iLayer = 0;

        private float[] m_arrYCompares = new float[] { 0 };
        private float[] m_arrZCompares = new float[] { 0, 180, 360 };
        private float[] m_arrXCompares = new float[] { 90, 270 };
        private int[] m_arrFlags = new int[] { 1, 1, 1 };

        private Color m_pXColor = new Color(1, 0, 0, 0.5f);
        private Color m_pYColor = new Color(0, 1, 0, 0.5f);
        private Color m_pZColor = new Color(0, 0, 1, 0.5f);
        private Color m_pSelectColor = new Color(1, 0.92f, 0.016f, 0.5f);

        private Matrix4x4[] m_arrMats = null;
        
        private EAxis m_eCurrentAxis = EAxis.None;
        private bool m_bTracking = false;
        private Vector3 m_stMousePosition = Vector3.zero;
        private Vector3 m_stInitialPos = Vector3.zero;

        void Awake()
        {
            if (RenderCamera == null)
            {
                RenderCamera = Camera.main;
            }

            m_iLayer = LayerMask.NameToLayer("Gzimos");

            m_arrMats = new Matrix4x4[]
            {
                Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90), Vector3.one),
                Matrix4x4.identity,
                Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90, 0, 0), Vector3.one)
            };

            CreateSphere();
            CreateXYZCricles();

            _scale();
        }

        private void CreateSphere()
        {
            m_pSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			m_pSphere.GetComponent<SphereCollider> ().isTrigger = true;
            m_pSphere.transform.SetParent(this.transform);
            m_pSphere.transform.position = m_stInitialPos;
            m_pSphere.transform.localScale = Vector3.one * m_fRadius * 1.98f;
            SphereCollider collider = m_pSphere.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            m_pSphere.layer = m_iLayer;
            MeshRenderer render = m_pSphere.GetComponent<MeshRenderer>();
            render.material = new Material(Shader.Find("TransparentShader"));
            render.material.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            
            //m_pSphere.GetComponent<MeshRenderer>().enabled = false;
        }

        private void CreateXYZCricles()
        {
            m_pMesh = new Mesh();
            m_pMesh.MarkDynamic();
            //创建xyz环
            //x
            int cnt = CreateArc(m_pXColor, m_fRadius, m_pMesh, m_arrMats[0]);
            m_pVtNums.Add(cnt);

            //y
            cnt = CreateArc(m_pYColor, m_fRadius, m_pMesh, m_arrMats[1]);
            m_pVtNums.Add(cnt);

            //z
            cnt = CreateArc(m_pZColor, m_fRadius, m_pMesh, m_arrMats[2]);
            m_pVtNums.Add(cnt);

            m_pMesh.Optimize();

            m_pMesh.normals = m_pMesh.vertices;
            m_pMesh.uv = new Vector2[m_pMesh.vertices.Length];
            m_pMesh.subMeshCount = 1;

            m_pMesh.SetIndices(SequentialTriangles(m_pMesh.vertices.Length), MeshTopology.Lines, 0);

            m_pMeshGo = new GameObject("Rotate");
            m_pMeshGo.transform.SetParent(m_pSphere.transform);
            m_pMeshGo.layer = m_iLayer;
            m_pMeshGo.AddComponent<MeshFilter>().mesh = m_pMesh;
            m_pMeshGo.AddComponent<MeshRenderer>().material = new Material(Shader.Find("RotateShader"));
            m_pMeshGo.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// unused
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="angle"></param>
        public void UpdateDynamicArc(float startAngle, float angle)
        {
            if (m_eCurrentAxis == EAxis.All || m_eCurrentAxis == EAxis.None)
            {
                return;
            }

            if (m_pDynamicArcGo == null)
            {
                m_pDynamicArcMesh = new Mesh();
                m_pDynamicArcMesh.MarkDynamic();
                m_pDynamicArcGo.transform.localPosition = Vector3.zero;
                m_pDynamicArcGo = new GameObject("arc");
                m_pDynamicArcGo.transform.SetParent(m_pSphere.transform);
                m_pDynamicArcGo.layer = m_iLayer;
                m_pDynamicArcGo.AddComponent<MeshFilter>().mesh = m_pDynamicArcMesh;
                m_pDynamicArcGo.AddComponent<MeshRenderer>().material = new Material(Shader.Find("RotateShader"));
            }

            //m_arrMats[(int)m_eCurrentAxis]
            CreateArc(new Color(0,0.5f,0.5f,0.5f), m_fRadius * 0.9f, m_pDynamicArcMesh, Matrix4x4.identity, angle * Mathf.Deg2Rad, true);

            m_pDynamicArcMesh.Optimize();

            m_pDynamicArcMesh.normals = m_pDynamicArcMesh.vertices;
            m_pDynamicArcMesh.uv = new Vector2[m_pDynamicArcMesh.vertices.Length];
            m_pDynamicArcMesh.subMeshCount = 1;
            
            m_pDynamicArcMesh.SetIndices(SequentialTriangles(m_pDynamicArcMesh.vertices.Length), MeshTopology.Lines, 0);
        }

        public void SetPosition(Vector3 pos)
        {
            m_stInitialPos = pos;
            if (m_pSphere != null)
            {
                this.m_pSphere.transform.position = pos;
            }
        }

        public void SetRotate(Vector3 euler)
        {
            this.m_pSphere.transform.localEulerAngles = euler;
        }

        public Vector3 GetRotate()
        {
			
            return this.m_pSphere.transform.localEulerAngles;
        }

        private void _scale()
        {
			var planes = new Plane (Camera.main.transform.forward,transform.position);
			float distance = planes.GetDistanceToPoint (Camera.main.transform.position);
			m_pSphere.transform.localScale =new Vector3(1, 1, 1) * (-distance/6);
           // m_pSphere.transform.localScale = Vector3.one * (Vector3.Distance(RenderCamera.transform.position, m_pSphere.transform.position) / 6);
        }

        private IEnumerator delayRelease()
        {
            yield return new WaitForEndOfFrame();

            if (OnRotateEnd != null)
            {
                OnRotateEnd(m_eCurrentAxis, GetRotate());
            }
        }
        
        private void Update()
        {
            _scale();

            if (Input.GetMouseButtonDown(0) && m_eCurrentAxis != EAxis.None)
            {
                m_stMousePosition = Input.mousePosition;
                m_bTracking = true;
            }
            if (Input.GetMouseButtonUp(0) && m_eCurrentAxis != EAxis.None)
            {
                SelectedAxis(EAxis.None);
                m_bTracking = false;

                StartCoroutine(delayRelease());
            }          

            if (!m_bTracking)
            {
                Camera cam = RenderCamera;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100 , 1 << m_iLayer))
                {
                    float rv, rh;
                    GetAngle(hit.point, out rv, out rh);

                    float angle = 0;
                    EAxis axis = FindClosetAxis(rv,rh,out angle);

                    SelectedAxis(axis);
                    if (OnAxisChanged != null)
                    {
                        OnAxisChanged(axis, angle);
                    }
                }
            }
            else if(m_eCurrentAxis != EAxis.None)
            {
                if (!EnableFreeRotate && m_eCurrentAxis == EAxis.All)
                {
                    //do nothing
                }
                else
                {
                    Vector3 delta = TrackBall(Input.mousePosition);
                    m_pSphere.transform.localRotation *= Quaternion.Euler(delta);
                    if (OnRotateChanged != null)
                    {
					
                        OnRotateChanged(m_eCurrentAxis, delta);
                    }
                }
            }
        }

        private Vector3 CompueteRotateDir(Vector3 offset,EAxis type)
        {
            if(offset.x + offset.y == 0)
            {
                return Vector3.zero;
            }
            Vector3 startPos = m_pSphere.transform.position;
            Vector3 start = Camera.main.WorldToScreenPoint(startPos);
            Vector3 axis;
            Vector3 end;
            if (type == EAxis.X)
            {
                end = Camera.main.WorldToScreenPoint(startPos + m_pSphere.transform.right * 2);
                axis = Vector3.right;
            }
            else if (type == EAxis.Y)
            {
                end = Camera.main.WorldToScreenPoint(startPos + m_pSphere.transform.up * 2);
                axis = Vector3.up;
            }
            else
            {
                end = Camera.main.WorldToScreenPoint(startPos + m_pSphere.transform.forward * 2);
                axis = Vector3.forward;
            }

            Vector3 axisDir = end - start;
            axisDir.Normalize();

            axisDir = Vector3.Cross(Vector3.forward, axisDir);

            float moveLen = axisDir.x * offset.x + axisDir.y * offset.y;
            
            float mx = moveLen * axis.x;
            float my = moveLen * axis.y;
            float mz = moveLen * axis.z;
            return new Vector3(mx, my, mz);
        }

        private Vector3 TrackBall(Vector3 screenPosition)
        {
            Vector3 offset = (screenPosition - m_stMousePosition) * RotateSpeed;
            m_stMousePosition = screenPosition;

            int yflag = m_arrFlags[1];
            int xflag = m_arrFlags[0];
            int zflag = m_arrFlags[2];

            Vector3 angle = Vector3.zero;
            if (m_eCurrentAxis == EAxis.All)
            {
                Vector3 dir = RenderCamera.transform.forward;
                Quaternion q = Quaternion.AngleAxis(offset.y,
                    m_pSphere.transform.InverseTransformVector(RenderCamera.transform.right));
                q *= Quaternion.AngleAxis(-offset.x,
                    m_pSphere.transform.InverseTransformVector(RenderCamera.transform.up));

                angle += q.eulerAngles;
            }
            else
            {
                angle += CompueteRotateDir(offset, m_eCurrentAxis);
            }

            return angle;
        }

        private EAxis FindClosetAxis(float rv,float rh,out float angle)
        {
            EAxis axis = EAxis.None;
            angle = 0;
            float dy = getMinAngle(rv, m_arrYCompares);
            if (dy <= SnapAngle)
            {
                angle = rv;
                axis = EAxis.Y;
            }

            float dz = getMinAngle(rh, m_arrZCompares);
            if (dz < SnapAngle)
            {
                if (axis == EAxis.Y)
                {
                    if (dz < dy)
                    {
                        angle = rh;
                        axis = EAxis.Z;
                    }
                }
                else
                {
                    angle = rh;
                    axis = EAxis.Z;
                }
            }

            float dx = getMinAngle(rh, m_arrXCompares);
            if (dx < SnapAngle)
            {
                if (axis == EAxis.Y)
                {
                    if (dx < dy)
                    {
                        angle = rh;
                        axis = EAxis.X;
                    }
                }
                else if (axis == EAxis.Z)
                {
                    if (dx < dz)
                    {
                        angle = rh;
                        axis = EAxis.X;
                    }
                }
                else
                {
                    angle = rh;
                    axis = EAxis.X;
                }
            }

            if (axis == EAxis.None)
            {
                axis = EAxis.All;
            }

            return axis;
        }

        private void GetAngle(Vector3 position, out float rv, out float rh)
        {
            Vector3 pt = m_pSphere.transform.InverseTransformPoint(position);
            Vector3 dir = pt - Vector3.zero;
            Vector3 v0 = dir.normalized;

            //vertical angle
            rv = Mathf.Asin(v0.y) * Mathf.Rad2Deg;
            v0.y = 0;
            v0 = v0.normalized;
            //horizontal angle
            rh = Vector3.Angle(Vector3.right, v0);
            if (rh < 0)
            {
                rh += 360;
            }
        }

        private void SelectedAxis(EAxis axis)
        {
            m_eCurrentAxis = axis;
            List<Color> colors = new List<Color>();
            SetColor(colors, m_eCurrentAxis == EAxis.X ? m_pSelectColor : m_pXColor, m_pVtNums[0]);
            SetColor(colors, m_eCurrentAxis == EAxis.Y ? m_pSelectColor : m_pYColor, m_pVtNums[1]);
            SetColor(colors, m_eCurrentAxis == EAxis.Z ? m_pSelectColor : m_pZColor, m_pVtNums[2]);
            m_pMesh.SetColors(colors);
        }

        private void SetColor(List<Color> colors, Color color, int count)
        {
            for(int i=0;i<count;i++)
            {
                colors.Add(color);
            }
        }

        private float getMinAngle(float angle,float[] compares)
        {
            float min = float.MaxValue;
            for(int i=0;i<compares.Length;i++)
            {
                float df = Mathf.Abs(angle - compares[i]);
                if(min > df)
                {
                    min = df;
                }
            }

            return min;
        }

        private int CreateArc(Color color, float radius, Mesh mesh, Matrix4x4 mat, float angle = 2*Mathf.PI, bool close = false)
        {
            if (m_fTheta < 0.0001f) m_fTheta = 0.0001f;

            int count = mesh.vertexCount;
            List<Vector3> vertices = new List<Vector3>(mesh.vertices);
            List<Color> colors = new List<Color>(mesh.colors);

            // draw circle
            Vector3 beginPoint = mat * Vector3.zero;
            Vector3 firstPoint = mat * Vector3.zero;
            for (float theta = 0; theta < angle; theta += m_fTheta)
            {
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);
                Vector3 endPoint = mat * new Vector3(x, 0, z);
                if (theta == 0 && !close)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    vertices.Add(beginPoint);
                    vertices.Add(endPoint);

                    colors.Add(color);
                    colors.Add(color);
                }
                beginPoint = endPoint;
            }

            // draw last line
            vertices.Add(firstPoint);
            vertices.Add(beginPoint);

            colors.Add(color);
            colors.Add(color);

            mesh.SetVertices(vertices);
            mesh.SetColors(colors);

            return vertices.Count - count;
        }

        private static int[] SequentialTriangles(int len)
        {
            int[] array = new int[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = i;
            }
            return array;
        }

        private static int[] TriangleStrips(int len)
        {
            int count = (len - 2) / 2;
            int[] array = new int[count];
            for (int i = 0; i < len - 2; i+=2)
            {
                int index = (i / 2) * 3;
                array[index] = 0;
                array[index + 1] = i + 1;
                array[index + 2] = i + 3;
            }
            return array;
        }
    }
