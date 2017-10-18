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

float GetAxonRadius(int axonID, float t) {
	float weight = axonInitDataCBuffer[axonID].weight;
	float baseRadius = abs(weight) * (maxAxonRadius - minAxonRadius) + minAxonRadius;

	float neuron0 = abs(neuronFeedDataCBuffer[axonInitDataCBuffer[axonID].fromID].curValue) * neuronInitDataCBuffer[axonInitDataCBuffer[axonID].fromID].radius;
	float neuron1 = abs(neuronFeedDataCBuffer[axonInitDataCBuffer[axonID].toID].curValue) * neuronInitDataCBuffer[axonInitDataCBuffer[axonID].toID].radius;

	float distToSideScreenEdge = (0.5 - min((1.0 - t), t));           // 0.5 at edge, 0.0 at middle of spline
	float flareMask = saturate(distToSideScreenEdge - 0.4) * 10 * 1;      // 0.0 --> 0.1 * 10 == 0->1,   1 at edge
	//if(distToSideScreenEdge < 0.3) {
	//	pixColor = lerp(bgColor, pixColor, smoothstep(0.0, 1.0, distToSideScreenEdge / 0.3));
	//}
	//float flareMask = min(clamp(1.0 - t, 0, 0.15), clamp(t, 0, 0.15));

	float pulseDistance = abs(t - axonSimDataCBuffer[axonID].pulsePos);
	float pulseMultiplier = ((1.0 - smoothstep(0, 0.2, pulseDistance)) + 1.0) * 2.0;

	float radius0 = neuron0 * baseRadius * pulseMultiplier;
	radius0 = lerp(radius0, neuron0 * 0.5, flareMask);
	float radius1 = neuron1 * baseRadius * pulseMultiplier;
	radius1 = lerp(radius1, neuron1 * 0.5, flareMask);

	return baseRadius;
	//return lerp(radius0, radius1, t);
}