import {AbsoluteCenter, Box, Button, HStack, Text, VStack} from '@chakra-ui/react'
import Link from 'next/link';

export default function Home() {
  return (
      <Box position="relative" h="100vh" bg="blue.200">
          <AbsoluteCenter>
              <VStack boxShadow="md" bg="whiteAlpha.900" p="10" rounded="md">
                  <Text fontSize="5xl">Welcome to Task Tracker</Text>
                  <HStack>
                      <Link href="/login" passHref>
                          <Button size="lg" colorScheme="blue.200" variant="outline">Login</Button>
                      </Link>

                      <Link href="/register" passHref>
                          <Button size="lg" colorScheme="blue">Register</Button>
                      </Link>
                  </HStack>
              </VStack>
          </AbsoluteCenter>
      </Box>

  );
}
