using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;


using System;
using System.IO;
using System.Threading.Tasks;

#if ENABLE_WINMD_SUPPORT
using Windows.Storage.Pickers;
using Windows.Storage;
#endif

[RequireComponent(typeof(Button))]
public class OpenJsonFileBrowser : MonoBehaviour, IPointerDownHandler {

    public GameObject JsonManager;
    JsonDataImport JDI;

    public void OnPointerDown(PointerEventData eventData) { }

	// Use this for initialization
	void Start () {
        JDI = JsonManager.GetComponent<JsonDataImport>();
        var button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
	}
	
	private async void onClick()
    {
#if ENABLE_WINMD_SUPPORT
        // Windows FileOpenPicker Implementation:

        string FilePath = await LoadFileAsync();
        Debug.Log(FilePath);
        JDI.localpath = FilePath;
#else
        // Standalone File Browser Implementation:
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "json", false);
        string FileLocation = new System.Uri(paths[0]).AbsoluteUri;
        JDI.localpath = FileLocation;
#endif
    }


    internal static async Task<string> LoadFileAsync()
    {
        {
#if ENABLE_WINMD_SUPPORT
            var pickCompleted = new TaskCompletionSource<string>();

            UnityEngine.WSA.Application.InvokeOnUIThread(
                async () =>
                {
                    Stream stream = null;
                    FileOpenPicker picker = new FileOpenPicker();
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    picker.FileTypeFilter.Add(".json");
                    picker.FileTypeFilter.Add("*");
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.CommitButtonText = "Select Data";

                    var file = await picker.PickSingleFileAsync();
                    string filePath = null;

                    if (file != null)
                    {
                        filePath = file.Path;
                    }
                    pickCompleted.SetResult(filePath);
                },
                true
            );

            await pickCompleted.Task;

            return (pickCompleted.Task.Result);

#else
        throw new InvalidOperationException(
            "Sorry, no file dialog support for other platforms here");
#endif
        }
    }
}
