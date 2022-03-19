/*
 *THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 *LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace OpenSim.Framework
{
	public class Autoipport
    {

		#region GetPublicIP
		/// Determine IP
		public static string GetPublicIP()
		{
			// Insert configuration url setting here
			string url = "http://checkip.dyndns.org";
			// or other server youreip.php.example
			//string url = "http://otherserver.com/youreip.php";
						
			System.Net.WebRequest req = System.Net.WebRequest.Create(url);
			System.Net.WebResponse resp = req.GetResponse();
			System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
			string response = sr.ReadToEnd().Trim();
			string[] a = response.Split(':');
			string a2 = a[1].Substring(1);
			string[] a3 = a2.Split('<');
			string a4 = a3[0];
			return a4;

			//test:
			// Insert configuration url setting here
			// or other server youreip2.php.example no parsing
			//string url = "http://checkip.amazonaws.com/";
			// or other server youreip2.php.example
			//string url = "http://otherserver.com/youreip2.php";

		}
		#endregion

		#region GetNextFreePort
		/// find the next available TCP port within a given range
		public static int GetNextFreePort(int min, int max)
		{
			// Insert configuration setting range here
			if (max < min)
				throw new ArgumentException("Max cannot be less than min.");

			var ipProperties = IPGlobalProperties.GetIPGlobalProperties();

			var usedPorts =
				ipProperties.GetActiveTcpConnections()
					.Where(connection => connection.State != TcpState.Closed)
					.Select(connection => connection.LocalEndPoint)
					.Concat(ipProperties.GetActiveTcpListeners())
					.Concat(ipProperties.GetActiveUdpListeners())
					.Select(endpoint => endpoint.Port)
					.ToArray();

			var firstUnused =
				Enumerable.Range(min, max - min)
					.Where(port => !usedPorts.Contains(port))
					.Select(port => new int?(port))
					.FirstOrDefault();

			if (!firstUnused.HasValue)
				throw new Exception($"All local TCP ports between {min} and {max} are currently in use.");

			return firstUnused.Value;
		}
		#endregion

		// testing lines here:

		// ### URL: ###
		// SYSTEMIP
		public string AUTOSYSTEMIP = GetPublicIP();
		// BaseHostname
		public string AutoBaseHostname = GetPublicIP();
		// BaseURL
		public string AutoBaseURL = GetPublicIP();

		// ### PORT: ###
		// http_listener_port
		public int AutoHttpListenerPort = GetNextFreePort(9000, 9099);
		// XmlRpcPort
		public int AutoXmlRpcPort = GetNextFreePort(20800, 20899);
		// InternalPort
		public int AutoInternalPort = GetNextFreePort(9100, 9199);

	}
}
