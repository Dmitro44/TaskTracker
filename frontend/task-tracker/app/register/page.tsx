"use client"

import {Box, Button, Center, Heading, Stack, Text} from "@chakra-ui/react";
import {Form, Formik} from "formik";
import {object, ref, string} from "yup";
import {InputField} from "@/app/components/InputField";
import {EmailIcon, InfoIcon, LockIcon} from "@chakra-ui/icons";
import Link from "next/link";
import {register} from "@/app/services/auth";
import {useRouter} from "next/navigation"

const schema = object().shape({
    username: string().required("Please enter your username"),
    firstName: string().required("Please enter your first name"),
    lastName: string().required("Please enter your last name"),
    email: string()
        .email("Invalid email address")
        .required("Please enter your email address"),
    password: string().required("Please enter your password"),
    confirmPassword: string()
        .oneOf([ref('password')], "Passwords must match")
        .required("Please confirm your password")
})

export default function RegisterPage() {
    const router = useRouter();

    return (
        <Center h="110vh" bg="blue.200" overflowY="auto" py="4">
            <Stack boxShadow="md" bg="whiteAlpha.900" p="20" rounded="md" overflowY="auto">
                <Heading as="h1">
                    Register
                </Heading>

                <Formik
                    validationSchema={schema}
                    onSubmit={ async (values, { setSubmitting }) => {
                        await register({
                            username: values.username,
                            firstName: values.firstName,
                            lastName: values.lastName,
                            email: values.email,
                            password: values.password,
                            confirmPassword: values.confirmPassword
                        })

                        setSubmitting(false);

                        router.push("/login");
                    }}
                    initialValues={{
                        username: "",
                        firstName: "",
                        lastName: "",
                        email: "",
                        password: "",
                        confirmPassword: ""
                    }}
                >
                    {({ isSubmitting }) => (
                        <Form>
                            <Stack my="4" gap="6">
                                <InputField
                                    name="username"
                                    type="text"
                                    label="Username"
                                    leftAddon={<InfoIcon color="blue.500"/>}
                                />
                                <InputField
                                    name="firstName"
                                    type="text"
                                    label="First Name"
                                    leftAddon={<InfoIcon color="blue.500"/>}
                                />
                                <InputField
                                    name="lastName"
                                    type="text"
                                    label="Last Name"
                                    leftAddon={<InfoIcon color="blue.500" />}
                                />
                                <InputField
                                    name="email"
                                    type="email"
                                    label="Email"
                                    leftAddon={<EmailIcon color="blue.500" />}
                                />
                                <InputField
                                    name="password"
                                    type="password"
                                    label="Password"
                                    leftAddon={<LockIcon color="blue.500" />}
                                />
                                <InputField
                                    name="confirmPassword"
                                    type="password"
                                    label="Confirm Password"
                                    leftAddon={<LockIcon color="blue.500" />}
                                />
                                <Button
                                    isLoading={isSubmitting}
                                    size="lg"
                                    colorScheme="blue"
                                    type="submit"
                                >
                                    Register
                                </Button>
                            </Stack>
                        </Form>
                    )}
                </Formik>

                <Text>
                    <span>Already have an account? </span>
                    <Link href="/login" passHref>
                        <Box as="span" color="blue.500" fontWeight="medium" _hover={{ color: "blue.700", textDecoration: "underline" }}>
                            Sign in
                        </Box>
                    </Link>
                </Text>
            </Stack>
        </Center>
    );
}