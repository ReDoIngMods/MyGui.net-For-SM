using MyGui.net.Properties;
using System.Net.Http.Headers;

namespace MyGui.net
{
	public partial class FormUpdater : Form
	{
		List<Form> previouslyVisibleForms = new List<Form>();
		private bool downloadCompleted = false;
		private CancellationTokenSource cancellationTokenSource;
		public FormUpdater(string url, Form[] closeAfter = null)
		{
			InitializeComponent();

			cancellationTokenSource = new CancellationTokenSource();

			if (closeAfter != null)
			{
				foreach (var item in closeAfter)
				{
					if (item != null && item.Visible)
					{
						previouslyVisibleForms.Add(item);
						item.Hide();
					}
				}
			}

			this.FormClosing += (s, e) => {
				foreach (var item in previouslyVisibleForms)
				{
					item?.Show();
				}
				cancellationTokenSource.Cancel();
			};

			DownloadFileAsync(url, Path.Combine(Application.ExecutablePath, "..", "Update.zip"), Settings.Default.UpdateBearerToken, cancellationTokenSource);
		}

		private async Task DownloadFileAsync(string url, string destinationPath, string bearerToken, CancellationTokenSource cts)
		{
			Util.httpClient.DefaultRequestHeaders.Add("User-Agent", "MyGui.NET");
			Util.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
			Util.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

			// Handle redirect (GitHub may redirect to raw content URL)
			try
			{
				using (HttpResponseMessage response = await Util.httpClient.GetAsync(url, cts.Token))
				{

					response.EnsureSuccessStatusCode();


					label1.Text = "Updating MyGui.NET...";
					// Get the actual content length from the headers
					long? totalBytes = response.Content.Headers.ContentLength;

					using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
									fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
					{
						var buffer = new byte[8192];
						long totalRead = 0;
						int bytesRead;

						while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
						{
							await fileStream.WriteAsync(buffer, 0, bytesRead);
							totalRead += bytesRead;

							// If we have the total size, calculate progress
							if (totalBytes.HasValue)
							{
								int progressPercentage = (int)((double)totalRead / totalBytes.Value * 100);
								progressBar.Value = progressPercentage;
								labelStatus.Text = $"Downloading: {totalRead / 1024:N0} KB / {totalBytes.Value / 1024:N0} KB ({progressPercentage}%)";
							}
							else
							{
								labelStatus.Text = $"Downloading: {totalRead / 1024:N0} KB";
							}
						}

						// Ensure that everything is written to the file
						await fileStream.FlushAsync();
					}

					downloadCompleted = true;
					labelStatus.Text = "Download completed!";
					progressBar.Value = 100;
					LaunchUpdater(destinationPath);
					return;
				}
			}
			catch (Exception e)
			{
				if (MessageBox.Show($"Error occured during download! Do you wish to retry?\nError: {e.Message}", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
				{
					DownloadFileAsync(url, Path.Combine(Application.ExecutablePath, "..", "Update.zip"), Settings.Default.UpdateBearerToken, cancellationTokenSource);
				}
				else
				{
					foreach (var item in previouslyVisibleForms)
					{
						item?.Show();
					}
					if (!this.IsDisposed && !this.Disposing)
					{
						this.Close();
					}
				}
				return;
			}
		}

		// Launches the updater batch file and exits the application.
		private void LaunchUpdater(string updateZipFile)
		{
			// Path for the batch script to be generated
			string updaterScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.bat");

			// Create the batch script content
			string batchScriptContent = @"
@echo off
SETLOCAL

:: Unzip the provided file to a temporary directory
set zipFile=%1
set tempDir=%~dp1temp\

:: Ensure temp directory exists
mkdir ""%tempDir%""

:: Extract files from the zip
powershell -command ""Expand-Archive -Path '%zipFile%' -DestinationPath '%tempDir%'"" 

:: Copy files to the parent directory
xcopy /E /I /H /Y ""%tempDir%\*"" ""%~dp1""

:: Cleanup: Delete the temporary extracted files
rd /S /Q ""%tempDir%""

:: Display success message to the user
echo Update completed successfully! 
timeout /t 3

:: Relaunch MyGui.exe from the parent directory
start """" ""%~dp1MyGui.net.exe""

:: Delete the .bat file and the .zip file after the update
del ""%zipFile%""
del ""%~dp0Updater.bat""

:: Exit the batch script
exit
";

			// Write the batch script to the file
			File.WriteAllText(updaterScript, batchScriptContent);

			// Check if the updater batch file was created successfully
			if (!File.Exists(updaterScript))
			{
				MessageBox.Show("Failed to create updater script!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// Start the updater batch file with the update ZIP file as an argument
			System.Diagnostics.Process.Start(updaterScript, $"\"{updateZipFile}\"");

			// Close the current .NET application immediately after starting the batch file
			Application.Exit();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
