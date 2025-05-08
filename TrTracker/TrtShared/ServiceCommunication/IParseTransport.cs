using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TrtShared.DTO;

namespace TrtShared.ServiceCommunication
{
    public interface IParseTransport
    {
        /// <summary>
        /// Publishes json DTO of the parsed file to Upload service
        /// </summary>
        /// <param name="jsonDto">Serialized in json string of DTO</param>
        public Task PublishParsedDtoAsync(string jsonDto);

        /// <summary>
        /// Subscribes to Upload service in order to get full file path
        /// </summary>
        /// <returns>Full file path of the file to parse</returns>
        public ChannelReader<string> PathReader { get; }
    }
}
