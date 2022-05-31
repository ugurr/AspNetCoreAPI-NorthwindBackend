using Core.Utilities.Interceptors;
using Core.Utilities.Messages;
using FluentValidation;
using System;
using System.Linq;
using Core.CrossCuttingConcerns.Validation;
using Castle.DynamicProxy;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect:MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new Exception(AspectMessages.WrongValidationType);
            }
            _validatorType = validatorType;

        }
        protected override void OnBefore(IInvocation invocation)
        {
            //aşağıdaki kod new ProductValidator() ile aynı işi yapıyor.
            var validator =(IValidator) Activator.CreateInstance(_validatorType);


            //aşagıdaki kod "ProductValidator : AbstractValidator<Product>" buradaki Product ı alıyor
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];

            var entities = invocation.Arguments.Where(t => t.GetType() == entityType);
            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
         }
    }
}
