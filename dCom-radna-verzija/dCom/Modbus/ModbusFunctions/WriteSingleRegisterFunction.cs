using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters p = (ModbusWriteCommandParameters)CommandParameters;

            byte[] request = new byte[5];

            request[0] = (byte)(p.OutputAddress >> 8);
            request[1] = (byte)(p.OutputAddress & 0xFF);

            request[2] = (byte)(p.Value >> 8);
            request[3] = (byte)(p.Value & 0xFF);

            request[4] = 0x00;

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort address = (ushort)((response[1] << 8) | response[2]);
            ushort value = (ushort)((response[3] << 8) | response[4]);

            result.Add(new Tuple<PointType, ushort>(PointType.HR_LONG, address), value);

            return result;
        }
    }
}