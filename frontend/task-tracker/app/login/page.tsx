"use client"

import {Box, Button, Center, Heading, Stack, Text} from "@chakra-ui/react";
import Link from "next/link";
import {Form, Formik} from "formik";
import {InputField} from "@/app/components/InputField";
import {EmailIcon, LockIcon} from "@chakra-ui/icons"
import {object, string} from "yup"
import {login} from "@/app/services/auth";

const schema = object().shape({
    email: string()
        .email("Invalid email address")
        .required("Please enter your email address"),
    password: string().required("Please enter your password"),
})

export default function LoginPage() {
    return (
        <Center h="100vh" bg="blue.200">
            <Stack boxShadow="md" bg="whiteAlpha.900" p="20" rounded="md" >
                <Heading as='h1'>
                    Log in
                </Heading>

                <Formik
                    validationSchema={schema}
                    onSubmit={ async (values, { setSubmitting }) => {
                        await login({
                            email: values.email,
                            password: values.password
                        });

                        setSubmitting(false);
                    }}
                    initialValues={{ email: "", password: "" }}
                >
                    {({ isSubmitting} ) => (
                        <Form>
                            <Stack my="4" gap="6">
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
                                    leftAddon={<LockIcon color="blue.500"/>}
                                />
                                <Button
                                    isLoading={isSubmitting}
                                    size="lg"
                                    colorScheme="blue"
                                    type="submit"
                                >
                                    Login
                                </Button>
                            </Stack>
                        </Form>
                    )}
                </Formik>

                <Text>
                    <span>Don&#39;t have an account? </span>
                    <Link href="/register" passHref>
                        <Box as="span" color="blue.500" fontWeight="medium" _hover={{ color: "blue.700", textDecoration: "underline" }}>
                            Sign up
                        </Box>
                    </Link>
                </Text>
            </Stack>
        </Center>
    );
}