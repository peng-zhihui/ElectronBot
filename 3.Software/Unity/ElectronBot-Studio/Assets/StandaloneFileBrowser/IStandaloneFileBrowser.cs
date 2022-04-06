using System;

namespace SFB {
    public interface IStandaloneFileBrowser {
        string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect);
        string[] OpenFolderPanel(string title, string directory, bool multiselect);
        string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions);

        void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb);
        void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb);
        void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb);
    }
}
