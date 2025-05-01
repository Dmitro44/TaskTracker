import {useField} from "formik";
import {FormControl, FormLabel, FormHelperText} from "@chakra-ui/react"
import {InputGroup, InputLeftAddon, Input} from "@chakra-ui/react";
import {ReactElement} from "react";

interface IInputFieldProps {
    name: string
    type: string
    label: string
    leftAddon: ReactElement

}

export const InputField = (props: IInputFieldProps) => {
    const {label, leftAddon, ...restOfProps} = props;
    const [field, meta] = useField(props);
    return (
        <FormControl id={props.name} isInvalid={!!meta.error && !!meta.touched}>
            {label && (
                <FormLabel mb="1" htmlFor={props.name}>
                    {label}
                </FormLabel>
            )}
            <InputGroup>
                {leftAddon && <InputLeftAddon bg="blue.50">{leftAddon}</InputLeftAddon>}
                <Input focusBorderColor="blue.50" {...field} {...restOfProps} />
            </InputGroup>
            {meta.error && meta.touched && (
                <FormHelperText>{meta.error}</FormHelperText>
            )}
        </FormControl>
    );
}