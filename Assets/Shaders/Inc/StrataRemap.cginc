// REQUIRES ALSO INCLUDING NOISESHARED.cginc !!!!

float GetRemappedValue01(float inputValue, float noiseOffset, float numStrata) {
	float remappedValue = saturate(inputValue) + saturate(Value1D(saturate(inputValue) + noiseOffset, numStrata).x) * (0.5 / (numStrata-1));
	return inputValue;
	//return remappedValue;
}