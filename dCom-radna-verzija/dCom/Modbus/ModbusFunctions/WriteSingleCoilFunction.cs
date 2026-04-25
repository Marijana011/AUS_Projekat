using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters p = (ModbusWriteCommandParameters)CommandParameters;

            byte[] request = new byte[6];

            //Function code 05
            request[0] = 5;

            //Addr
            request[1] = (byte)(p.OutputAddress >> 8);
            request[2] = (byte)(p.OutputAddress & 0xFF);

            //Value
            if (p.Value == 1)
            {
                request[3] = 0xFF;
                request[4] = 0x00;
            }
            else 
            {
                request[3] = 0x00;
                request[4] = 0x00;
            }
            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> result =new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort address = (ushort)((response[1] << 8) | response[2]);

            ushort value = (response[3] == 0xFF) ? (ushort)1 : (ushort)0;

            result.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address), value);
        
            return result;
        }
    }
}