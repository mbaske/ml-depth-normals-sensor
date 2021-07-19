using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents.Sensors;

namespace MBaske.Sensors
{
    [RequireComponent(typeof(Camera))]
    public class DepthNormalsSensorComponent : SensorComponent, IDisposable
    {
        /// <summary>
        /// Name of the generated <see cref="RenderTextureSensor"/>.
        /// Note that changing this at runtime does not affect how the Agent sorts the sensors.
        /// </summary>
        public string SensorName
        {
            get { return m_SensorName; }
            set { m_SensorName = value; }
        }
        [SerializeField]
        private string m_SensorName = "DepthNormalsSensorComponent";

        /// <summary>
        /// Compression type for the render texture observation.
        /// </summary>
        public SensorCompressionType CompressionType
        {
            get { return m_Compression; }
            set { m_Compression = value; UpdateSensor(); }
        }
        [SerializeField]
        SensorCompressionType m_Compression = SensorCompressionType.PNG;

        /// <summary>
        /// Whether to stack previous observations. Using 1 means no previous observations.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int ObservationStacks
        {
            get { return m_ObservationStacks; }
            set { m_ObservationStacks = value; }
        }
        [SerializeField]
        [Range(1, 50)]
        [Tooltip("Number of frames that will be stacked before being fed to the neural network.")]
        private int m_ObservationStacks = 1;

        /// <summary>
        /// Width of the generated observation.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        [SerializeField]
        private int m_Width = 84;

        /// <summary>
        /// Height of the generated observation.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        [SerializeField]
        private int m_Height = 84;

        [SerializeField]
        private Material m_Material;

        [Space]
        [SerializeField]
        private RawImage m_UI;
       
        private RenderTextureSensor m_Sensor;

        /// <inheritdoc/>
        public override ISensor[] CreateSensors()
        {
            Dispose();
            m_Sensor = new RenderTextureSensor(InitTexture(), false, SensorName, m_Compression);
            if (ObservationStacks != 1)
            {
                return new ISensor[] { new StackingSensor(m_Sensor, ObservationStacks) };
            }
            return new ISensor[] { m_Sensor };
        }

        private RenderTexture InitTexture()
        {
            if (m_Material == null)
            {
                m_Material = new Material(Shader.Find("Custom/DepthNormals"));
            }
            
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode |= DepthTextureMode.DepthNormals;
            cam.targetTexture = new RenderTexture(Width, Height, 16, RenderTextureFormat.ARGB32); 

            if (m_UI != null)
            {
                m_UI.texture = cam.targetTexture;
            }
            return cam.targetTexture;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_Material != null)
            {
                Graphics.Blit(source, destination, m_Material, 0);
            }
        }

        /// <summary>
        /// Update fields that are safe to change on the Sensor at runtime.
        /// </summary>
        internal void UpdateSensor()
        {
            if (m_Sensor != null)
            {
                m_Sensor.CompressionType = m_Compression;
            }
        }

        /// <summary>
        /// Clean up the sensor created by CreateSensors().
        /// </summary>
        public void Dispose()
        {
            if (m_Sensor != null)
            {
                m_Sensor.Dispose();
                m_Sensor = null;
            }
        }
    }
}
