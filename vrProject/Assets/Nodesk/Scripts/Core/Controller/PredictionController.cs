using System.Collections.Generic;
using UnityEngine;

namespace Nodesk.Scripts.Core.Controller
{
    public class PredictionController : MonoBehaviour
    {
        [SerializeField] private GameObject contents;
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private AutocompleteBasedSimilarity autocompleteBasedSimilarity;
    
        private int _cursorIndex = -1;
        private List<WordController> wordControllers = new List<WordController>();

        private AbstractAdapter _abstractAdapter;

        public void Show()
        {
            if (wordControllers.Count != 0)
            {
                foreach (var wordController in wordControllers)
                {
                    Destroy(wordController.gameObject);
                }
            }
        
            wordControllers.Clear();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (wordControllers.Count != 0)
            {
                foreach (var wordController in wordControllers)
                {
                    Destroy(wordController.gameObject);
                }
            }
        
            wordControllers.Clear();
            gameObject.SetActive(false);
        }

        public bool IsShow()
        {
            return gameObject.activeSelf;
        }
    
        public void RunPrediction(AbstractAdapter adapter)
        {
            var split = adapter.GetText().Split(' ');
            var word = split[split.Length - 1];
            var words = autocompleteBasedSimilarity.DoAutocomplete(word);
            if (!string.IsNullOrWhiteSpace(word))
            {
                words.Insert(0, word);    
            }
            SetWordList(words);

            _abstractAdapter = adapter;
        }
    
        private void SetWordList(List<string> words)
        {
            if (wordControllers.Count != 0)
            {
                foreach (var wordController in wordControllers)
                {
                    Destroy(wordController.gameObject);
                }
            }
        
            wordControllers.Clear();
            foreach (var word in words)
            {
                var go = Instantiate(textPrefab, contents.transform);
                var wordController = go.GetComponent<WordController>();
                wordController.SetWord(word);
                wordControllers.Add(wordController);
            }

            _cursorIndex = 0;
            for (int i = 0; i < wordControllers.Count; i++)
            {
                if (i == _cursorIndex)
                {
                    wordControllers[i].Select();
                }
                else
                {
                    wordControllers[i].UnSelect();
                }
            }
        }

        public void ControllerInputOnTouchEvent(InputData inputData)
        {
            var stickDown = inputData.Action == InputAction.SelectDown;
            var stickUp = inputData.Action == InputAction.SelectUp;
            var select = inputData.Action == InputAction.Select;
        
            if (stickDown)
            {
                DownCursor();
            }
            else if(stickUp)
            {
                UpCursor();
            } else if (select)
            {
                SelectWord();
            }
        }

        private void UpCursor()
        {
            _cursorIndex--;
            if (_cursorIndex < 0)
            {
                _cursorIndex = wordControllers.Count - 1;
            }
        
            for (int i = 0; i < wordControllers.Count; i++)
            {
                if (i == _cursorIndex)
                {
                    wordControllers[i].Select();
                }
                else
                {
                    wordControllers[i].UnSelect();
                }
            }
        }
    
        private void DownCursor()
        {
            _cursorIndex++;
            if (_cursorIndex >= wordControllers.Count)
            {
                _cursorIndex =  0;
            }
        
            for (int i = 0; i < wordControllers.Count; i++)
            {
                if (i == _cursorIndex)
                {
                    wordControllers[i].Select();
                }
                else
                {
                    wordControllers[i].UnSelect();
                }
            }
        }

        private void SelectWord()
        {
            if (wordControllers.Count == 0) return;
        
            var word = wordControllers[_cursorIndex].GetWord();
            var startIndex = _abstractAdapter.GetText().LastIndexOf(' ');
            if (startIndex == -1)
            {
                startIndex = 0;
            }
            var endIndex = _abstractAdapter.GetText().Length - 1;
            _abstractAdapter.ProcessPredictionSelect(startIndex, endIndex, word);
        } 
    }
}
