using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using Windows.Media.SpeechRecognition;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

using System.Diagnostics;
using Windows.UI.Popups;

namespace NightNight
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();

            try
            {
                StorageFile sf = await Package.Current.InstalledLocation.GetFileAsync(@"voicecmds.xml");
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(sf);
                MessageDialog dialog = new MessageDialog("We got the voice command Initiated");
                await dialog.ShowAsync();


            } catch (Exception ex)
            {
                Debug.Write("Failed to register custom voice commands because: " + ex.Message);
            }
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            // Handle when app is launched by Cortana
            if (e.Kind == ActivationKind.VoiceCommand)
            {
                VoiceCommandActivatedEventArgs commandArgs = e as VoiceCommandActivatedEventArgs;
                SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;
                IReadOnlyList<string> recognizedVoiceCommandPhrases;

                System.Diagnostics.Debug.WriteLine("voiceCommandName: " + voiceCommandName);
                System.Diagnostics.Debug.WriteLine("textSpoken: " + textSpoken);

                switch (voiceCommandName)
                {
                    case "SpyProtocol":
                        System.Diagnostics.Debug.WriteLine("Spy Protocol 224225 activated");
                        break;

                    case "Change_Temperature":
                        string temperature = "";

                        if (speechRecognitionResult.SemanticInterpretation.Properties.TryGetValue("temperature", out recognizedVoiceCommandPhrases))
                        {
                            temperature = recognizedVoiceCommandPhrases.First();
                        }

                        System.Diagnostics.Debug.WriteLine("Change_Temperature command. The passed PhraseTopic value is " + temperature);
                        break;

                    case "Change_Light_Color":
                        string color = "";

                        if (speechRecognitionResult.SemanticInterpretation.Properties.TryGetValue("colors", out recognizedVoiceCommandPhrases))
                        {
                            color = recognizedVoiceCommandPhrases.First();
                        }

                        System.Diagnostics.Debug.WriteLine("Change_Light_Color command. The passed PhraseList value is " + color);
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown command");
                        break;
                }
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
