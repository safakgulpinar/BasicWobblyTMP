using TMPro;
using UnityEngine;

namespace _BasicWobblyTMP.Scripts
{
    public class BasicWobblyTMP : MonoBehaviour
    {
        [SerializeField] private WobblyTextType wobblyTextType;
        [SerializeField, Range(0f, 20f)] private float waveOffset;

        private TMP_Text _textMeshPro;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _wordIndexes;
        private int[] _wordLengths;

        private void Awake()
        {
            _textMeshPro = GetComponent<TMP_Text>();
            InitializeWordIndexes();
        }

        private void Update()
        {
            _textMeshPro.ForceMeshUpdate();
            ApplyEffect();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetText("Hello World! I'm Safak");
            }
        }

        private void ApplyEffect()
        {
            switch (wobblyTextType)
            {
                case WobblyTextType.WaveOne:
                    ApplyWaveEffectOne();
                    break;
                case WobblyTextType.WaveTwo:
                    ApplyWaveEffectTwo();
                    break;
                case WobblyTextType.WaveThree:
                    ApplyWaveEffectThree();
                    break;
                case WobblyTextType.WaveFour:
                    ApplyWaveEffectFour();
                    break;
            }
        }

        private void ApplyWaveEffectOne()
        {
            var textInfo = _textMeshPro.textInfo;
            for (var i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                _vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                for (var j = 0; j < 4; j++)
                {
                    var orig = _vertices[charInfo.vertexIndex + j];
                    _vertices[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2 + orig.x * 0.01f) * waveOffset, 0);
                }
            }

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                _textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        private void ApplyWaveEffectTwo()
        {
            _mesh = _textMeshPro.mesh;
            _vertices = _mesh.vertices;

            for (var i = 0; i < _vertices.Length; i++)
            {
                var offset = CalculateOffset(Time.time + i);
                _vertices[i] += offset;
            }

            _mesh.vertices = _vertices;
            _textMeshPro.canvasRenderer.SetMesh(_mesh);
        }

        private void ApplyWaveEffectThree()
        {
            _mesh = _textMeshPro.mesh;
            _vertices = _mesh.vertices;

            for (var i = 0; i < _textMeshPro.textInfo.characterCount; i++)
            {
                var charInfo = _textMeshPro.textInfo.characterInfo[i];
                var index = charInfo.vertexIndex;
                var offset = CalculateOffset(Time.time + i * waveOffset);

                for (var j = 0; j < 4; j++)
                {
                    _vertices[index + j] += offset;
                }
            }

            _mesh.vertices = _vertices;
            _textMeshPro.canvasRenderer.SetMesh(_mesh);
        }

        private void ApplyWaveEffectFour()
        {
            _mesh = _textMeshPro.mesh;
            _vertices = _mesh.vertices;

            for (var w = 0; w < _wordIndexes.Length; w++)
            {
                var wordIndex = _wordIndexes[w];
                var offset = CalculateOffset(Time.time + w * waveOffset);

                for (var i = 0; i < _wordLengths[w]; i++)
                {
                    var characterInfo = _textMeshPro.textInfo.characterInfo[wordIndex + i];
                    var index = characterInfo.vertexIndex;

                    for (var j = 0; j < 4; j++)
                    {
                        _vertices[index + j] += offset;
                    }
                }
            }

            _mesh.vertices = _vertices;
            _textMeshPro.canvasRenderer.SetMesh(_mesh);
        }

        private Vector3 CalculateOffset(float time)
        {
            return new Vector3(Mathf.Sin(time * 3.3f), Mathf.Cos(time * waveOffset), 0);
        }

        /// <summary>
        /// Sets the text of the TMP component and initializes word indexes for wavy effects.
        /// </summary>
        /// <param name="text">The text to set.</param>
        public void SetText(string text)
        {
            _textMeshPro.text = text;
            InitializeWordIndexes();
        }

        private void InitializeWordIndexes()
        {
            var text = _textMeshPro.text;
            var wordCount = text.Split(' ').Length;
            _wordIndexes = new int[wordCount];
            _wordLengths = new int[wordCount];

            var wordStartIndex = 0;
            var wordIndex = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ' && i != text.Length - 1) continue;
                _wordIndexes[wordIndex] = wordStartIndex;
                _wordLengths[wordIndex] = i - wordStartIndex + (i == text.Length - 1 && text[i] != ' ' ? 1 : 0);
                wordStartIndex = i + 1;
                wordIndex++;
            }
        }

        private enum WobblyTextType
        {
            WaveOne,
            WaveTwo,
            WaveThree,
            WaveFour
        }
    }
}
