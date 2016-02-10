namespace Xceno.CustomUsers.Filters
{
	using System;
	using System.ComponentModel.DataAnnotations;

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class NotEqualToAttribute : ValidationAttribute
	{
		private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";

		public NotEqualToAttribute(string otherProperty)
		  : base(DefaultErrorMessage)
		{
			if ( string.IsNullOrEmpty(otherProperty) )
			{
				throw new ArgumentNullException("otherProperty");
			}

			this.OtherProperty = otherProperty;
		}

		public string OtherProperty { get; private set; }

		public override string FormatErrorMessage(string name)
		{
			return string.Format(this.ErrorMessageString, name, this.OtherProperty);
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if ( value != null )
			{
				var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(this.OtherProperty);
				var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

				if ( value.Equals(otherPropertyValue) )
				{
					return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
				}
			}

			return ValidationResult.Success;
		}
	}
}
