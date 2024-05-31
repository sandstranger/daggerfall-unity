﻿using System;
using Mono.CSharp;
using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class AndroidRootView : MonoBehaviour
    {
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private Button _copyGameContentButton;
        [SerializeField]
        private Button _exitGameButton;
        [SerializeField]
        private GameObject _progressBar;

        private AndroidRootController _viewController = new AndroidRootController();

        private void Awake()
        {
            _startGameButton.onClick.AddListener(async ()=>
            {
                _progressBar.SetActive(true);
                await _viewController.StartGame();
            });

            _copyGameContentButton.onClick.AddListener(async () =>
            {
                _progressBar.SetActive(true);
                await _viewController.CopyStreamingAssetsToInternalMemory();
                _progressBar.SetActive(false);
            });

            _exitGameButton.onClick.AddListener(_viewController.QuitGame);
        }
    }
}