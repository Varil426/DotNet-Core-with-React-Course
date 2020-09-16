import React, { useContext } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { combineValidators, isRequired } from "revalidate";
import { Button, Form } from "semantic-ui-react";
import ErrorMessage from "../../app/common/form/ErrorMessage";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import TextInput from "../../app/common/form/TextInput";
import { IEditFormValues } from "../../app/models/profile";
import { RootStoreContext } from "../../app/stores/rootStore";

const validate = combineValidators({
	displayName: isRequired("Display Name"),
});

interface IProps {
	editProfile: (values: IEditFormValues) => void;
}

const ProfileEditForm: React.FC<IProps> = ({ editProfile }) => {
	const rootStore = useContext(RootStoreContext);
	const { profile } = rootStore.profileStore;
	return (
		<FinalForm
			onSubmit={editProfile}
			validate={validate}
			initialValues={profile}
			render={({
				handleSubmit,
				submitting,
				submitError,
				invalid,
				pristine,
				dirtySinceLastSubmit,
			}) => (
				<Form onSubmit={handleSubmit} error>
					<Field
						name="displayName"
						component={TextInput}
						placeholder={profile?.displayName}
					/>
					{submitError && !dirtySinceLastSubmit && (
						<ErrorMessage
							error={submitError}
							text="Display Name cannot be empty"
						/>
					)}
					<Field
						name="bio"
						component={TextAreaInput}
						placeholder="Bio"
					/>

					<Button
						disabled={
							(invalid && !dirtySinceLastSubmit) || pristine
						}
						loading={submitting}
						color="teal"
						content="Submit"
						fluid
					/>
				</Form>
			)}
		></FinalForm>
	);
};

export default ProfileEditForm;
