#include "Assets/Shaders/Inc/NoiseShared.cginc"

struct NeuronInitData {
    float radius;
    float type;  // in/out/hidden
    float age;
};
struct NeuronFeedData {
    float curValue;  // [-1,1]  // set by CPU continually
};
struct NeuronSimData {
    float3 pos;
};
struct AxonInitData {  // set once at start
    float weight;
    int fromID;
    int toID;
};
struct AxonSimData {
    float3 p0;
    float3 p1;
    float3 p2;
    float3 p3;
    float pulsePos;
};

struct CurveSample {
	float3 origin;
	float3 normal;
	float3 right;
	float3 up;	
	float3 forward;
};

StructuredBuffer<NeuronInitData> neuronInitDataCBuffer;
StructuredBuffer<NeuronFeedData> neuronFeedDataCBuffer;
RWStructuredBuffer<NeuronSimData> neuronSimDataCBuffer;
StructuredBuffer<AxonInitData> axonInitDataCBuffer;
RWStructuredBuffer<AxonSimData> axonSimDataCBuffer;

float minAxonRadius = 0.05;
float maxAxonRadius = 0.5;
float minNeuronRadius = 0.05;
float maxNeuronRadius = 0.5;

float neuronAttractForce = 0.004;
float axonStraightenForce = .02;
float neuronRepelForce = 2.0;
float axonRepelForce = 0.2;

float time = 0.0;

// CUBIC:
float3 GetPoint (float3 p0, float3 p1, float3 p2, float3 p3, float t) {
	t = saturate(t);
	float oneMinusT = 1.0 - t;
	return oneMinusT * oneMinusT * oneMinusT * p0 +	3.0 * oneMinusT * oneMinusT * t * p1 + 3.0 * oneMinusT * t * t * p2 +	t * t * t * p3;
}
// CUBIC
float3 GetFirstDerivative (float3 p0, float3 p1, float3 p2, float3 p3, float t) {
	t = saturate(t);
	float oneMinusT = 1.0 - t;
	return 3.0 * oneMinusT * oneMinusT * (p1 - p0) + 6.0 * oneMinusT * t * (p2 - p1) + 3.0 * t * t * (p3 - p2);
}

float GetNeuronRadius(int neuronID, float3 dir) {

	//float radius = min(max(neuronInitDataCBuffer[neuronID].radius * abs(neuronFeedDataCBuffer[neuronID].curValue), minNeuronRadius), maxNeuronRadius) * 3;

	float interp = smoothstep(0, 1, abs(neuronFeedDataCBuffer[neuronID].curValue));
	float radius = lerp(minNeuronRadius, maxNeuronRadius, interp);
	//float3 origin = neuronSimDataCBuffer[neuronID].pos;

	float extrudeNoiseFreq = 1.5;
	float extrudeNoiseAmp = 0.0;
	float3 extrudeNoiseOffset = float3(0.14, 0.73, 0.52) * time * 0.6;

	float noiseExtrude = (Value3D(dir + neuronID + extrudeNoiseOffset, extrudeNoiseFreq).x + 0.25) * radius;  // add neuronID so noise varies from neuron to neuron
	radius += noiseExtrude * extrudeNoiseAmp;

	return radius;
}

float GetAxonRadius(int axonID, float t, float angle) {
	
	//// Clean up this function to return radius at uv coordinate / sample position
	//// Convert old BrainCompute code to utilize these two functions
	//// debug


	float weight = axonInitDataCBuffer[axonID].weight;
	float baseRadius = abs(weight) * (maxAxonRadius - minAxonRadius) + minAxonRadius;

	////////////////////////////////////////////////////////////////////////////
	float extrudeNoiseFreq = 0.33;

	float noiseExtrude = (Value3D(float3(axonID, t, angle), extrudeNoiseFreq).x + 0.5) * baseRadius;
	baseRadius += noiseExtrude * 1.5;
	////////////////////////////////////////////////////////////////////////////

	//float neuron0 = abs(neuronFeedDataCBuffer[axonInitDataCBuffer[axonID].fromID].curValue) * neuronInitDataCBuffer[axonInitDataCBuffer[axonID].fromID].radius;
	//float neuron1 = abs(neuronFeedDataCBuffer[axonInitDataCBuffer[axonID].toID].curValue) * neuronInitDataCBuffer[axonInitDataCBuffer[axonID].toID].radius;

	float neuronRadius0 = GetNeuronRadius(axonInitDataCBuffer[axonID].fromID, float3(0,1,0));
	float neuronRadius1 = GetNeuronRadius(axonInitDataCBuffer[axonID].toID, float3(0,1,0));

	float closestNeuronRadius = lerp(neuronRadius0, neuronRadius1, round(t));

	float distToSideScreenEdge = (0.5 - min((1.0 - t), t));           // 0.5 at edge, 0.0 at middle of spline
	float flareMask = saturate(distToSideScreenEdge - 0.4) * 10 * 1;      // 0.0 --> 0.1 * 10 == 0->1,   1 at edge

	float pulseDistance = abs(t - axonSimDataCBuffer[axonID].pulsePos);
	float pulseMultiplier = ((1.0 - smoothstep(0, 0.2, pulseDistance)) + 1.0) * 1.0;

	baseRadius *= pulseMultiplier;

	baseRadius = lerp(baseRadius, closestNeuronRadius * 0.5, flareMask);
	
	return baseRadius;
}

CurveSample GetAxonSample(int axonID, float t, float angle) {

	CurveSample curveSample;

	float noiseFreq = 0.14;
	float noiseAmp = 1.5;
	float3 noiseOffset = float3(0.5, 0.25, 0.9) * time * 10.0;

	float distToSideScreenEdge = (0.5 - min((1.0 - t), t)) + 0.1;           // 0.5 at edge, 0.0 at middle of spline

	float3 ringOrigin = GetPoint(axonSimDataCBuffer[axonID].p0, axonSimDataCBuffer[axonID].p1, axonSimDataCBuffer[axonID].p2, axonSimDataCBuffer[axonID].p3, t);
	ringOrigin += Value3D(ringOrigin + noiseOffset, noiseFreq).yzw * noiseAmp * (1.0 - distToSideScreenEdge * 2.0);

	float3 forward = normalize(GetFirstDerivative(axonSimDataCBuffer[axonID].p0, axonSimDataCBuffer[axonID].p1, axonSimDataCBuffer[axonID].p2, axonSimDataCBuffer[axonID].p3, t));
	float3 right = normalize(cross(forward, float3(0.0, 1.0, 0.0)));
	float3 up = normalize(cross(right, forward));

	// Spiral Offset!!!:::::
	float spiralFreq = 20.0;
	float spiralAmp = 1;
	float spiralRight = cos(t * spiralFreq) * spiralAmp;
	float spiralUp = sin(t * spiralFreq) * spiralAmp;
	ringOrigin += right * spiralRight + up * spiralUp;

	float x = cos((angle) * 2.0 * 3.14159);
	float y = sin((angle) * 2.0 * 3.14159);
		
	curveSample.normal = right * x + up * y;
	curveSample.origin = ringOrigin;
	curveSample.right = right;
	curveSample.up = up;
	curveSample.forward = forward;
	
	return curveSample;
}