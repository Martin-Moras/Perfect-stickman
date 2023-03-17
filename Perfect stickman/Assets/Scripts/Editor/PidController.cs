/// <summary>
/// A (P)roportional, (I)ntegral, (D)erivative Controller
/// </summary>
/// <remarks>
/// The controller should be able to control any process with a
/// measureable value, a known ideal value and an input to the
/// process that will affect the measured value.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/PID_controller"/>
public sealed class PidController
{
	private float currentValue = 0;

	public PidController(float GainProportional, float GainIntegral, float GainDerivative)
	{
		this.GainProportional = GainProportional;
		this.GainIntegral = GainIntegral;
		this.GainDerivative = GainDerivative;
	}

	/// <summary>
	/// The controller output
	/// </summary>
	/// <param name="timeSinceLastUpdate">timespan of the elapsed time
	/// since the previous time that ControlVariable was called</param>
	/// <returns>Value of the variable that needs to be controlled</returns>
	public float ControlVariable(float timeSinceLastUpdate)
	{
		float error = (TargetValue - CurrentValue + 540) % 360 - 180;

		// integral term calculation
		IntegralTerm += (GainIntegral * error * timeSinceLastUpdate);

		// derivative term calculation
		float dInput = currentValue - last_currentValue;
		float derivativeTerm = GainDerivative * (dInput / timeSinceLastUpdate);

		// proportional term calcullation
		float proportionalTerm = GainProportional * error;

		float output = proportionalTerm + IntegralTerm - derivativeTerm;

		return output;
	}



	/// <summary>
	/// The proportional term produces an output value that
	/// is proportional to the current error value
	/// </summary>
	/// <remarks>
	/// Tuning theory and industrial practice indicate that the
	/// proportional term should contribute the bulk of the output change.
	/// </remarks>
	public float GainProportional { get; set; } = 0;
	/// <summary>
	/// The integral term is proportional to both the magnitude
	/// of the error and the duration of the error
	/// </summary>
	public float GainIntegral { get; set; } = 0;
	/// <summary>
	/// The derivative term is proportional to the rate of
	/// change of the error
	/// </summary>
	public float GainDerivative { get; set; } = 0;
	public float CurrentValue
	{
		get { return currentValue; }
		set
		{
			last_currentValue = currentValue;
			currentValue = value;
		}
	}
	public float TargetValue { get; set; } = 0;

	/// <summary>
	/// Adjustment made by considering the accumulated error over time
	/// </summary>
	/// <remarks>
	/// An alternative formulation of the integral action, is the
	/// proportional-summation-difference used in discrete-time systems
	/// </remarks>
	public float IntegralTerm { get; private set; } = 0;
	public float last_currentValue { get; private set; } = 0;

	/// <summary>
	/// Limit a variable to the set OutputMax and OutputMin properties
	/// </summary>
	/// <returns>
	/// A value that is between the OutputMax and OutputMin properties
	/// </returns>
	/// <remarks>
	/// Inspiration from http://stackoverflow.com/questions/3176602/how-to-force-a-number-to-be-in-a-range-in-c
	/// </remarks>

}