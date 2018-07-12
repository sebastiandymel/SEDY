﻿using System.Collections.Generic;

namespace FakeIMC.Server
{
    internal class Constants
    {
        public static readonly List<decimal> SweepFrequencies = new List<decimal>
        {
            100.000m,102.920m,105.925m,109.018m,112.202m,115.478m,118.850m,122.321m,125.893m,129.569m,133.352m,
            137.246m,141.254m,145.378m,149.624m,153.993m,158.489m,163.117m,167.880m,172.783m,177.828m,183.021m,
            188.365m,193.865m,199.526m,205.353m,211.349m,217.520m,223.872m,230.409m,237.137m,244.062m,251.189m,
            258.523m,266.073m,273.842m,281.838m,290.068m,298.538m,307.256m,316.228m,325.462m,334.965m,344.747m,
            354.813m,365.174m,375.837m,386.812m,398.107m,409.732m,421.697m,434.010m,446.684m,459.727m,473.151m,
            486.968m,501.187m,515.822m,530.884m,546.387m,562.341m,578.762m,595.662m,613.056m,630.957m,649.382m,
            668.344m,687.860m,707.946m,728.618m,749.894m,771.792m,794.328m,817.523m,841.395m,865.964m,891.251m,
            917.276m,944.061m,971.628m,1000.000m,1029.201m,1059.254m,1090.184m,1122.018m,1154.782m,1188.502m,
            1223.20m,1258.925m,1295.687m,1333.521m,1372.461m,1412.538m,1453.784m,1496.236m,1539.927m,1584.893m,
            1631.17m,1678.804m,1727.826m,1778.279m,1830.206m,1883.649m,1938.653m,1995.262m,2053.525m,2113.489m,
            2175.20m,2238.721m,2304.093m,2371.374m,2440.619m,2511.886m,2585.235m,2660.725m,2738.420m,2818.383m,
            2900.68m,2985.383m,3072.557m,3162.278m,3254.618m,3349.654m,3447.466m,3548.134m,3651.741m,3758.374m,
            3868.12m,3981.072m,4097.321m,4216.965m,4340.103m,4466.836m,4597.270m,4731.513m,4869.675m,5011.872m,
            5158.22m,5308.844m,5463.865m,5623.413m,5787.620m,5956.621m,6130.558m,6309.573m,6493.816m,6683.439m,
            6878.59m,7079.458m,7286.182m,7498.942m,7717.915m,7943.282m,8175.230m,8413.951m,8659.643m,8912.509m,
            9172.75m,9440.609m,9716.280m,10000.000m
        };

        public static readonly List<decimal> TargetOegValues = new List<decimal>
        {
            0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0.001m,0.002m,0.003m,0.005m,0.008m,0.012m,0.019m,
            0.028m,0.041m,0.058m,0.079m,0.105m,0.139m,0.179m,0.224m,0.276m,0.334m,0.397m,0.463m,0.535m,0.611m,0.686m,0.764m,
            0.84m,0.925m,1.002m,1.085m,1.168m,1.249m,1.331m,1.415m,1.499m,1.583m,1.666m,1.747m,1.829m,1.905m,1.977m,2.048m,
            2.111m,2.172m,2.224m,2.271m,2.308m,2.339m,2.363m,2.38m,2.391m,2.397m,2.4m,2.4m,2.399m,2.397m,2.396m,2.4m,2.408m,
            2.423m,2.447m,2.483m,2.533m,2.604m,2.696m,2.819m,2.976m,3.172m,3.414m,3.706m,4.058m,4.459m,4.932m,5.461m,6.056m,
            6.7m,7.384m,8.111m,8.862m,9.633m,10.402m,11.16m,11.903m,12.612m,13.274m,13.882m,14.438m,14.929m,15.347m,15.695m,
            15.969m,16.173m,16.313m,16.392m,16.415m,16.39m,16.322m,16.216m,16.078m,15.914m,15.727m,15.516m,15.286m,15.037m,
            14.768m,14.476m,14.159m,13.814m,13.437m,13.024m,12.575m,12.084m,11.552m,10.978m,10.363m,9.714m,9.031m,8.321m,
            7.596m,6.864m,6.131m,5.41m,4.711m,4.048m,3.426m,2.855m,2.339m,1.883m,1.486m,1.148m,0.866m,0.635m,0.447m,0.297m,
            0.177m,0.063m,0.031m,0.02m
        };

        public static readonly decimal AmountOfSteps = 25m;

        public static int[] ReagLevels = {50, 65, 80};
    }
}